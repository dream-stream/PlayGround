import asyncio
import msvcrt
from nats.aio.client import Client as NATS
from threading import Thread, Event, Timer

counter = 0

def f(f_stop):
    if not f_stop.is_set():
        print(f"Counter: {counter}")
        Timer(1, f, [f_stop]).start()


async def run(loop, counter):
    nc = NATS()

    await nc.connect("localhost:4222", loop=loop)

    async def message_handler(msg):
        subject = msg.subject
        reply = msg.reply
        data = msg.data.decode()
        global counter
        counter += 1000
        # print("Received a message on '{subject}': {data}".format(
        #     subject=subject, data=data))

    # Simple publisher and async subscriber via coroutine.
    sid = await nc.subscribe("foo", cb=message_handler)

    print("The consumer is ready to receive messages")
    print("Press enter to close the application")

    while True:
        await asyncio.sleep(1, loop=loop)
        if msvcrt.kbhit():
            if ord(msvcrt.getch()) == 13:
                break

    # Remove interest in subscription.
    await nc.unsubscribe(sid)

    # Terminate connection to NATS.
    await nc.close()

if __name__ == '__main__':
    stopFlag = Event()
    f(stopFlag)
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run(loop, counter))
    loop.close()
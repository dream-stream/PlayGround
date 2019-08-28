import asyncio
import msvcrt
from nats.aio.client import Client as NATS

async def run(loop):
    nc = NATS()

    await nc.connect("localhost:4222", loop=loop)

    async def message_handler(msg):
        subject = msg.subject
        reply = msg.reply
        data = msg.data.decode()
        print("Received a message on '{subject} {reply}': {data}".format(
            subject=subject, reply=reply, data=data))

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
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run(loop))
    loop.close()
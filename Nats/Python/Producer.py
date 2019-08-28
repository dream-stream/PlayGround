import asyncio
from nats.aio.client import Client as NATS

async def run(loop):
    nc = NATS()

    await nc.connect("localhost:4222", loop=loop)
    await nc.publish("foo", b'Hello')
    await nc.publish("foo", b'World')
    await nc.publish("foo", b'!!!!!')

    # Terminate connection to NATS.
    await nc.close()

if __name__ == '__main__':
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run(loop))
    loop.close()
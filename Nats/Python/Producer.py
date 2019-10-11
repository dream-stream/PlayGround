import asyncio
from nats.aio.client import Client as NATS
import json

bah = {
    "test": [
    ]
}

async def run(loop):
    nc = NATS()

    await nc.connect("localhost:4222", loop=loop)
    
    for target_list in range(0, 1000):
        bah["test"].append({"Address": "Address", "LocationDescription": "Description", "SensorType": "Sensor", "Measurement": 20, "Unit": "Unit"})

    test = json.dumps(bah).encode()

    while(True):
        nc.publish("foo", test)


    # Terminate connection to NATS.
    await nc.close()

if __name__ == '__main__':
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run(loop))
    loop.close()
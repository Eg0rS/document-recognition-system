import asyncio

from aiokafka import AIOKafkaConsumer

from config.kafka_config import KafkaConfig


class Consumer:
    def __init__(self, config: KafkaConfig):
        self.config = config

    async def __consume(self):
        self.consumer = AIOKafkaConsumer(
            self.config.get_consumer_topic(),
            bootstrap_servers=self.config.get_host() + ':' + self.config.get_port(),
            auto_offset_reset='earliest',
            enable_auto_commit=True,
            group_id='request-consumer-group',
            auto_commit_interval_ms=1000,
            value_deserializer=lambda x: x.decode('utf-8')
        )

        await self.consumer.start()
        print("consumer started")
        try:
            async for msg in self.consumer:
                print("consumed: ", msg.topic, msg.partition, msg.offset,
                      msg.key, msg.value, msg.timestamp)
                await self.consumer.commit()
        finally:
            await self.consumer.stop()

    def start_consume(self):
        asyncio.run(self.__consume())

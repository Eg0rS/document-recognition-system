from config.app_config import AppConfig
from config.kafka_config import KafkaConfig
from kafka.consumer import Consumer
from kafka.producer import Producer


def main():
    config = AppConfig()
    kafka_config = config.get_kafka_config()

    kafka_consumer = Consumer(KafkaConfig(kafka_config))
    kafka_producer = Producer(KafkaConfig(kafka_config))

    kafka_consumer.start_consume()


if __name__ == '__main__':
    main()

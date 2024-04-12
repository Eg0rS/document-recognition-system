class KafkaConfig(object):

    def __init__(self, dict):
        self.kafka_host = dict.get('host')
        self.kafka_port = dict.get('port')
        self.kafka_consumer_topic = dict.get('kafka_consumer_topic')
        self.kafka_producer_topic = dict.get('kafka_producer_topic')

    def get_host(self):
        return self.kafka_host

    def get_port(self):
        return self.kafka_port

    def get_consumer_topic(self):
        return self.kafka_consumer_topic

    def get_producer_topic(self):
        return self.kafka_producer_topic

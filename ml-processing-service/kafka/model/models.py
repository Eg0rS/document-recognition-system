class Resolution:
    # есть ли нарушения
    is_violation = ''
    # список найденных нарушений
    violations = []
    # регистрационный номер автомобиля совершившего нарушение
    plate = ''

    def __init__(self, **kwargs):
        self.is_violation = kwargs['is_violation']
        self.plate = kwargs['plate']
        self.violations = kwargs['violations']


class Request:
    # id запроса на нарушение
    id = ''
    # id файла с нарушением
    file_id = ''

    def __init__(self, **kwargs):
        self.id = kwargs['id']
        self.file_id = kwargs['file_id']

    def get_id(self):
        return self.id

    def get_file_id(self):
        return self.file_id


class Response:
    # id запроса на нарушение
    id = ''
    # id файла с размеченным нарушением
    file_id = ''
    # резолюция по нарушению
    resolution = {}

    def __init__(self, **kwargs):
        self.id = kwargs['']
        self.file_id = kwargs['']
        self.resolution = kwargs['resolution']

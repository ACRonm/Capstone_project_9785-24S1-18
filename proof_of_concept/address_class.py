class Address:
    def __init__(self, id, number, street, unit, long, lat, city, postcode, region, accuracy):
        self.id = id
        self.number = number
        self.street = street
        self.unit = unit
        self.long = long
        self.lat = lat
        self.city = city
        self.postcode = postcode
        self.region = region
        self.accuracy = accuracy

    def get_full_address(self):
        return f"{self.unit} {self.number} {self.street}, {self.city}, {self.region}, {self.postcode}"

    def to_list(self):
        return [self.id, self.number, self.street, self.unit, self.long, self.lat, self.city, self.postcode, self.region, self.accuracy]

    def set_id(self, id):
        self.id = id

    # get address by id

    def get_address_by_id(self, id, addresses):

        for address in addresses:
            if address.id == id:
                return address
        else:
            return "Address not found"

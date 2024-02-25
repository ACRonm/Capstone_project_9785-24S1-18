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

    def set_id(self, id):
        self.id = id

    # set the street attribute
    def set_number(self, number):
        self.number = number

    def set_street(self, street):
        self.street = street

    # set the city attribute
    def set_city(self, city):
        self.city = city

    # set the region attribute
    def set_region(self, region):
        self.region = region

    def set_postcode(self, postcode):
        self.postcode = postcode

    def set_unit(self, unit):
        self.unit = unit

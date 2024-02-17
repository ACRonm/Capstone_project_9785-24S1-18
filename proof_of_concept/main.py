import csv

import Menu


def main():
    # Path to the data file
    data_file = './data/au.csv'

    menu = Menu.Menu()
    menu.run()

    fields = ['id', 'number', 'street', 'unit', 'lon',
              'lat', 'city', 'postcode', 'region', 'accuracy']

    addresses = []

    # Read the first 100 lines
    with open(data_file, 'r') as file:
        reader = csv.reader(file)
        headers = next(reader)
        print(headers)
        for i in range(100):
            # append to the addresses list
            addresses.append(next(reader))
            print(next(reader))

    address_dicts = [dict(zip(fields, address)) for address in addresses]

    for address_dict in address_dicts:
        print(address_dict)


    # TODO: Implement your text repair system logic here
if __name__ == '__main__':
    main()

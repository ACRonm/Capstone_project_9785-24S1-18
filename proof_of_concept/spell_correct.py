import fuzzywuzzy.process
import address_class
import Menu
from tqdm import tqdm


def correct_spelling(misspelt_addresses, correct_addresses):
    repaired_addresses = []
    wrong_addresses = []

    def postcode_is_within_10(postcode1, postcode2):
        if not postcode1 or not postcode2:
            return False
        return abs(int(postcode1) - int(postcode2)) < 10

    # Create a dictionary to store the correct addresses by postcode for faster lookup
    correct_addresses_by_postcode = {}
    for address in correct_addresses:
        if address.postcode not in correct_addresses_by_postcode:
            correct_addresses_by_postcode[address.postcode] = []
        correct_addresses_by_postcode[address.postcode].append(address)

    for misspelt_address in tqdm(misspelt_addresses):
        # use fuzzy wuzzy extractOne to find the closest match for both city and street

        # Filter the addresses by postcode for faster lookup
        filtered_addresses = correct_addresses_by_postcode.get(
            misspelt_address.postcode, []) if misspelt_address.postcode else correct_addresses

        # If the postcode was incorrectly entered, we will use all correct addresses for matching
        if not postcode_is_within_10(misspelt_address.postcode, filtered_addresses[0].postcode if filtered_addresses else None):
            filtered_addresses = correct_addresses

        # find the closest match for the street
        closest_street_match = fuzzywuzzy.process.extractOne(
            misspelt_address.street, [address.street for address in filtered_addresses], scorer=fuzzywuzzy.fuzz.ratio)

        # find the closest match for the city
        closest_city_match = fuzzywuzzy.process.extractOne(
            misspelt_address.city, [address.city for address in filtered_addresses], scorer=fuzzywuzzy.fuzz.ratio)

        # if the closest match for both the street and city are above a certain threshold, we will consider the address to be repaired
        if closest_street_match and closest_city_match and closest_street_match[1] > 66 and closest_city_match[1] > 66:

            # create a new address object with the corrected street and city
            repaired_address = misspelt_address

            repaired_address.city = closest_city_match[0]
            repaired_address.street = closest_street_match[0]

            repaired_addresses.append(repaired_address)
        else:

            # print the whole address and the closest match for both street and city
            print("\nMisspelt name: ", misspelt_address.street)
            print("\nMisspelt city: ", misspelt_address.city)

            print("\n Address: ", misspelt_address.get_full_address())

            print("\nClosest street match: ", closest_street_match)
            print("\nClosest city match: ", closest_city_match)

            wrong_addresses.append(misspelt_address)

    return repaired_addresses, wrong_addresses


def check_results(repaired_addresses, correct_addresses):

    misspelt_addresses = Menu.load_into_memory('./data/au_misspelt.csv')

    number_of_repaired_addresses = len(repaired_addresses)
    number_of_wrong_addresses = 0

    for repaired_address in repaired_addresses:
        for correct_address in correct_addresses:
            if repaired_address.id == correct_address.id:
                if repaired_address.street != correct_address.street or repaired_address.city != correct_address.city:
                    number_of_wrong_addresses += 1
                    print(
                        f"\nAddress {repaired_address.id} was not repaired correctly.")
                    print(f"Original: {correct_address.get_full_address()}")
                    print(f"Repaired: {repaired_address.get_full_address()}")
                    print(f"Misspelt: {repaired_address.get_address_by_id(
                        repaired_address.id, misspelt_addresses).get_full_address()}")
                    print("\n")

                break

    print(
        f"Number of repaired addresses: {number_of_repaired_addresses}")
    print(f"Number of wrong addresses: {number_of_wrong_addresses}")


def main():

    return


if __name__ == "__main__":
    main()

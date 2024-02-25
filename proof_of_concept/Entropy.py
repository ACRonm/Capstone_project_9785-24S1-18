import random
from fuzzywuzzy import fuzz
import string


def apply_entropy(addresses):

    misspelling_rate = 0.2
    misspelt_addresses = []

    for address in addresses:
        address = address.strip()
        if random.random() < misspelling_rate:
            # transpose two characters
            if len(address) > 1:
                index = random.randint(0, len(address) - 2)
                address = address[:index] + address[index +
                                                    1] + address[index] + address[index + 2:]

        misspelt_addresses.append(address)
    return misspelt_addresses


def string_match(misspelt_addresses, addresses):

    corrected_addresses = []
    number_of_incorrect = 0

    for address in misspelt_addresses:
        closest_match = max(
            addresses, key=lambda x: fuzz.ratio(x, address))
        corrected_addresses.append(closest_match)
        print("Confidence: ", fuzz.ratio(address, closest_match))
        print(f"Original: {address} -> Corrected: {closest_match}")

        address = closest_match
        if address != closest_match:
            number_of_incorrect += 1

    print("Number of addresses processed: ", len(addresses))
    print(f"Number of incorrect addresses: {number_of_incorrect}")
    print(f"Accuracy: {100 - (number_of_incorrect / len(addresses)) * 100}%")


def simulate_typing_errors(address):
    # Define operation functions
    def random_insertion(address):
        rand_index = random.randint(0, len(address))
        rand_char = random.choice(string.ascii_letters)
        return address[:rand_index] + rand_char + address[rand_index:]

    def random_deletion(address):
        if len(address) == 0:
            return address
        rand_index = random.randint(0, len(address) - 1)
        return address[:rand_index] + address[rand_index + 1:]

    def random_substitution(address):
        if len(address) == 0:
            return address
        rand_index = random.randint(0, len(address) - 1)
        rand_char = random.choice(string.ascii_letters)
        return address[:rand_index] + rand_char + address[rand_index + 1:]

    def random_transposition(address):
        if len(address) < 2:
            return address
        rand_index = random.randint(0, len(address) - 2)
        return address[:rand_index] + address[rand_index + 1] + address[rand_index] + address[rand_index + 2:]

    # Choose a random operation to apply
    operations = [random_insertion, random_deletion,
                  random_substitution, random_transposition]

    rand_operation = random.choice(operations)

    return rand_operation(address)

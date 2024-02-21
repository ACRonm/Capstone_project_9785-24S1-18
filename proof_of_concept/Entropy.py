import random
import Menu
import fuzzywuzzy
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
            addresses, key=lambda x: fuzzywuzzy.fuzz.ratio(x, address))
        corrected_addresses.append(closest_match)
        print("Confidence: ", fuzzywuzzy.fuzz.ratio(address, closest_match))
        print(f"Original: {address} -> Corrected: {closest_match}")

        # compare the original address with the corrected address
    for i in range(len(addresses)):
        if addresses[i] != corrected_addresses[i]:
            number_of_incorrect += 1

    rate_of_incorrect = number_of_incorrect / len(addresses)
    print(f"Rate of incorrect addresses: %{rate_of_incorrect*100:.2f}")


def simulate_typing_errors(s):
    # Define operation functions
    def random_insertion(s):
        rand_index = random.randint(0, len(s))
        rand_char = random.choice(string.ascii_letters)
        return s[:rand_index] + rand_char + s[rand_index:]

    def random_deletion(s):
        if len(s) == 0:
            return s
        rand_index = random.randint(0, len(s) - 1)
        return s[:rand_index] + s[rand_index + 1:]

    def random_substitution(s):
        if len(s) == 0:
            return s
        rand_index = random.randint(0, len(s) - 1)
        rand_char = random.choice(string.ascii_letters)
        return s[:rand_index] + rand_char + s[rand_index + 1:]

    def random_transposition(s):
        if len(s) < 2:
            return s
        rand_index = random.randint(0, len(s) - 2)
        return s[:rand_index] + s[rand_index + 1] + s[rand_index] + s[rand_index + 2:]

    # Choose a random operation to apply
    operations = [random_insertion, random_deletion,
                  random_substitution, random_transposition]
    rand_operation = random.choice(operations)

    return rand_operation(s)


if __name__ == "__main__":
    addresses = ["Fake", "Real", "Imaginary", "Nonexistent", "Fictional",
                 "Fictitious", "Phantom", "Fanciful", "Mythical", "Legendary", "Mythological"]

    misspelt_addresses = apply_entropy(addresses)

    string_match(misspelt_addresses, addresses)

    misspelt_addresses = []

    for address in addresses:
        misspelt_addresses.append(simulate_typing_errors(address))

    string_match(misspelt_addresses, addresses)

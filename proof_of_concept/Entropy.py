import random
import string


def simulate_errors(address_string):

    # Define operation functions
    def random_insertion(address_string):
        rand_index = random.randint(0, len(address_string))
        rand_char = random.choice(string.ascii_letters)
        return address_string[:rand_index] + rand_char + address_string[rand_index:]

    def random_deletion(address_string):
        if len(address_string) == 0:
            return address_string
        rand_index = random.randint(0, len(address_string) - 1)
        return address_string[:rand_index] + address_string[rand_index + 1:]

    def random_substitution(address_string):
        if len(address_string) == 0:
            return address_string
        rand_index = random.randint(0, len(address_string) - 1)
        rand_char = random.choice(string.ascii_letters)
        return address_string[:rand_index] + rand_char + address_string[rand_index + 1:]

    def random_transposition(address_string):
        if len(address_string) < 2:
            return address_string
        rand_index = random.randint(0, len(address_string) - 2)
        return address_string[:rand_index] + address_string[rand_index + 1] + address_string[rand_index] + address_string[rand_index + 2:]

    def ocr_error(address_string):
        if len(address_string) == 0:
            return address_string
        # ocr errors are a common source of errors in address data
        # we will simulate this by randomly replacing a character with a similar looking character
        # for example, 0 with O, 1 with I, 2 with Z, etc.
        # we will also add some random noise to the address

        # Define a dictionary of similar looking characters
        similar_chars = {
            '0': ['O'],
            '1': ['I'],
            '2': ['Z'],
            '3': ['B'],
            '4': ['A'],
            '5': ['S'],
            '6': ['G'],
            '7': ['T'],
            '8': ['B'],
            '9': ['Q']
        }

        # Choose a random character to replace
        rand_index = random.randint(0, len(address_string) - 1)
        rand_char = address_string[rand_index]

        # If the character is in the similar_chars dictionary, replace it with a similar looking character
        if rand_char in similar_chars:
            rand_char = random.choice(similar_chars[rand_char])

        # Add some random noise to the address
        noise = ''.join(random.choices(
            string.ascii_letters + string.digits, k=5))

        address_string = address_string[:rand_index] + \
            rand_char + address_string[rand_index + 1:] + noise

        return address_string

    # Choose a random operation to apply
    operations = [random_insertion, random_deletion,
                  random_substitution, random_transposition, random_transposition, ocr_error, ocr_error, ocr_error]

    rand_operation = random.choice(operations)

    return rand_operation(address_string)

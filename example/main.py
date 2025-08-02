import python_interface
def main():
    class1 = python_interface.create_class1(5)
    print(python_interface.get_class1_value(class1))
    print(class1)

    generic1 = python_interface.get_generic_record("Hello")
    file = open("generic1.txt", "w")
    generic2= python_interface.get_generic_record(file)
    print(generic2)
    print(generic1)
    file.close()


if __name__ == "__main__":
    main()

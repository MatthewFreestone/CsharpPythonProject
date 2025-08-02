from typing import Any
from pythonnet import load
load("coreclr")

import clr
from importlib.resources import files
path = files("python_interface.lib").joinpath("Library.dll")
clr.AddReference(str(path))



from Library import Class1, Class1Record, GenericRecord

def hello() -> str:
    return "Hello from python_interface!"

def create_class1(value: int) -> Class1:
    return Class1(value)

def get_class1_value(instance: Class1) -> int:
    return instance.GetValue()

def get_generic_record(value: Any) -> Any:
    return GenericRecord(value)

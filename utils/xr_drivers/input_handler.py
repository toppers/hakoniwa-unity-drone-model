from abc import ABC, abstractmethod

class InputHandler(ABC):
    @abstractmethod
    def handle_input(self):
        pass

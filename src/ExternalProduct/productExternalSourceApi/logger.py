import logging
import logging.config
import json


class StructuredMessage:
    def __init__(self, message, /, **kwargs):
        self.message = message
        self.kwargs = kwargs

    def __str__(self):
        return "%s >>> %s" % (self.message, json.dumps(self.kwargs, default=str))


_ = StructuredMessage  # optional, to improve readability

# Define the logging configuration
logging.basicConfig(
    level=logging.DEBUG,  # Set the logging level
    filename="/logs/app.log",  # Log file path
    filemode="w",  # Mode to open the log file ('w' for overwrite, 'a' for append)
    format="%(message)s",
)

# Create a logger
logger = logging.getLogger(__name__)

logger.error(_("Error", extra={"error": "aboba"}))

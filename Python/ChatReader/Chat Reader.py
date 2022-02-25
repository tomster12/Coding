

# Chat Size Large
# Chat Scale 150
localDir = "C:/Users/tombu/Files/Coding/py/ChatReader/"
from PIL import Image, ImageEnhance, ImageFilter
import pyscreenshot as ImageGrab
import pytesseract
import time


def bitmapImage(image):
    image = image.convert("RGBA")
    newDatas = []
    curDatas = image.getdata()
    for data in curDatas:
        if data[0] > 80 or data[1] > 80 or data[2] > 80:
            newDatas.append((255, 255, 255))
        else:
            newDatas.append((0, 0, 0))
    image.putdata(newDatas)
    return image


def getTextFromImage(image):
    output = pytesseract.image_to_string(image, config="--psm 11")
    outputList = output.split("\n\n")
    return outputList


def getImage():
    im = ImageGrab.grab(bbox=(0, 800, 1500, 1030))
    return im


im = getImage()
im = bitmapImage(im)
im.save(localDir + "test.png")
output = getTextFromImage(im)

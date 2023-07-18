import os
from PIL import Image, ImageEnhance

def makeBW():
    for filename in os.listdir("bw"):
        f = os.path.join(os.getcwd(), filename)
        if os.path.isfile(f):
            im = Image.open(f)
            im_2 = im.convert('LA')
            enhancer = ImageEnhance.Brightness(im_2)
            im_output = enhancer.enhance(0.5)
            im_output.save("bw/" + filename)
            print(filename + " has been saved")
        else:
            print(f + " IS NOT FOUND")



makeBW()
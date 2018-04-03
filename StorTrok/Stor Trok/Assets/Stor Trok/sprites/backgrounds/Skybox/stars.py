# generates a picture with stars

import pygame, random

pygame.init()
anzStars = 500
for a in range(0, 6):
    myPic = pygame.surface.Surface([2048, 2048])
    myPic.fill([0, 0, 0])
    for i in xrange(anzStars):
        pygame.draw.rect(myPic, (255, 255, 255), [random.randint(1, 2048), random.randint(1, 2048), 4, 4])
    pygame.image.save(myPic, "starsPic"+str(a)+".jpg")
pygame.quit()

# bk0010-stopexpress
Porting game Stop the Express to BK-0010, work in progress.


The ZX Spectrum game was developed by Hudson Soft and originally released in 1983.
In 1990, an unknown programmer in Lviv, Ukraine, ported the game to the Elektronika MS-0515, a PDP-11-like computer based on a T11 clone.
In 2017, I ported the game to the UKNC.
Finally, in September 2018, I began porting the game to the BK-0010 machine.

The real challenge was to fit 32K game into 16K RAM available on BK-0010.
So I had to strip down the game: demo mode sequence removed, reduced number of tiles and re-worked the way the tiles drawn, removed locomotion tiles used on the last scene of the game,
the info screen shown once at the beginning only and the memory reused after that, and finally I removed all sound effects.


Screenshots of the ported version:

![](screenshot/titlescreen.png)

![](screenshot/demoscreen.png)

Tiles used:

![](screenshot/newtiles.png)

Screenshot of the original game on MS-0515:

![](screenshot/original-ms0515.png)


#### See Also

 - [Porting Stop the Express to UKNC](https://github.com/nzeemin/uknc-stopexpress)
 - [Porting BK game Lode Runner to UKNC](https://github.com/nzeemin/uknc-loderunner)

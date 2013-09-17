Aftermath v0.0.1
============

A [Roguelike](http://roguebasin.roguelikedevelopment.org/index.php?title=Berlin_Interpretation) zombie survival game
written in C# using Monogame. The game is very early in development.

For the latest release (msi installer) [see the releases page](https://github.com/Azathotep/Aftermath/releases)

-----

![Screenshot](https://github.com/Azathotep/Aftermath/raw/master//Images/screenshot1.png)

The city has been overrun by zombies! You are one of the few survivors.

Game vision:

 - Sandbox simulation of a zombie outbreak in a large city environment.
 - A large and interesting city layout made up of roads and buildings, uniquely generated with each new game.
 - Thousands (as many as packs the city) zombies wandering the city.
 - Different zombie types (but not too many).
 - Simple controls and items.
 - Landmarks and special buildings like hospitals, airports, prisons, etc.
 - Everything in the world is persisted. Zombies do not spawn in nearby and vanish when out of sight.
 - Loot shops and homes to find ammunition, weapons, food and medical supplies.
 - Build a safehouse to protect self while sleeping or from zombie attacks.
 - Active events such as zombie horde attacks (similar to L4D) and attacks on the player's safehouse.
 - Customizable game options.
 - A storyline and clues concerning the cause of the outbreak and a possible cure, which the player is under no obligation to follow.

Ideas for gameplay options:
 - UK rules. Guns and ammunition are rare. More reliance on melee weapons and running away.
 - US rules. Abundant guns and ammunition.
 - On The Same Team. Survivors do not attack each other.
 - Realistic Damage. Hostile survivors can kill you in one shot with weapons.
 - Not Immune. One zombie bite is fatal.
 - Afraid of the Dark. Game is always in daytime.

Building
======

 - The game has a dependency on Monogame, which in turn has a dependency on SharpDX, which in turn has a dependency on DirectX. The last one is a matter of updating DirectX. For the other two dependencies either install a binary release of Monogame or build from the latest dev trunk. Either way you need a fairly new version of Monogame for it to install the Windows DirectX project type into Visual Studio. I am currently building Monogame off the fork I took (https://github.com/Azathotep/MonoGame)
 - The game installer uses WiX but presumably if you are building the game you care about the installer.

# Arthur's Intelligence Agency

Neha Thumu, Claire Lu

## Game Overview
In this game, you play as an employee of Arthur's Intelligence Agency, a company that creates artworks for various customers. You operate a special machine that absorbs existing art and spits out pieces for you to arrange into new art.

## Major Milestones
1. Alpha -- Setup, Crowd Sim, Request System
2. Beta -- Jigsaw mechanic, Jigsaw materials, Requests + Jigsaw Integration
3. Final -- Machine Progression, Sketchbook Event, Day Cycle, Game Logic Polishing
   
## Significant Difficulties
**1. Creating "uneven" jigsaw pieces**

The jigsaw puzzle is a core mechanic of the game. Whenever the player fulfills a request, they must create new art by using the machine to generate the pieces of the art and then arrange it. This is the jigsaw puzzle. Because it is repeated many times throughout the game, we wanted our implementation for jigsaw puzzles to be able to generate uneven shaped puzzle pieces. This would help introduce variation to the kinds of puzzle the player would encounter. By uneven, we mean that puzzle pieces are not necessarily square, their widths and heights can be different from each other, and the horizontal edges can be diagonal. We also visually wanted the horizontal edges of adjacent puzzle pieces to flow from one to the other, avoiding jutting corners that stick out.

Here's a reference image from the game Florence. 

<img width="200" height="591" alt="image" src="https://github.com/user-attachments/assets/6e3c699b-1cb1-40a5-b62c-7741fbe92fdb" />

Many of the online resources we found on creating jigsaw puzzles focused on square, very gridlike puzzles. The main two resources we looked at are [here](https://youtu.be/9mSwbMiV3lU?si=hLj-cyWk0WKXMJFg) and [here](https://youtu.be/rgm3nityU-M?si=U9nsOdXCqtZmj7hu). The first one implements Bezier curves to create the shape of the puzzle piece's edges. However, the implementation for creating the jigsaw board relies on all edges having the same shape, as it simply uses rotations & reflections of the single Bezier curve. The second resource also creates square pieces and does not cover creating the tabs and indents of a puzzle piece.

These two resources had very different implementations and we didn't anticipate that creating a jigsaw puzzle would be a major time sink for our project. However, after testing the first implementation (and finding the Bezier curves overly complicated for our purposes) we ended up following the second resource as a foundation, and subsequently spending a lot of time figuring out how to implement all the other visual elements we wanted our puzzle to have.

Some challenges along the way:
- Initially, following the second resource, we created the pieces of the puzzle by creating Quad meshes and tiling a material with a test image. By setting the Tiling and Offset of the material to certain values corresponding to the piece's location relative to the puzzle, we were able to set each piece's material to the correct section of the whole image. However, we introduced uneven sized pieces by making each of the columns in a puzzle variable widths (where a column is defined as a vertical stacks of puzzle pieces). Even thought the image would still look seamless, it caused the image to look stretched for bigger columns and squeezed for smaller columns. Thus, rather than adjusting Tiling and Offset of the material, we decided to create the Mesh through code and properly set the UVs.
- However, finding the correct vertex coordinates and setting the UVs was not very simple. Following our reference image from Florence, many of the pieces are trapezoidal. For bottom edge pieces and top edge pieces, it was easy to know what the vertex corners were because the bottom two or top two would always be aligned with x=-0.5 or x=0.5 when visualizing the piece at the origin. Since our implementation defined the heights of the sides of the trapezoid, it was easy to find the other two vertex corners. However, because of the trapezoidal nature of all the pieces, there was no way to tell what the vertex corners were for the middle pieces. We ended up keeping track of the accumulated height of the heights of each side of the columns in order to determine the proper placement of vertex corners for each piece. The vertex corners were converted to UV space by putting them in world space and converting to screen space. This allowed for correct material setup on uneven pieces. Below are some initial attempts.
   <img width="500" height="566" alt="Screenshot 2025-06-27 194254" src="https://github.com/user-attachments/assets/15a2217e-32de-41d0-9021-34543029067e" />

   <img width="500" height="645" alt="Screenshot 2025-06-27 193948" src="https://github.com/user-attachments/assets/cde3a07c-5729-44f5-8325-ad7ba3d6138d" />

   And an example with the correct UVs:

   <img width="500" height="569" alt="Screenshot 2025-06-28 151614" src="https://github.com/user-attachments/assets/937d9f3e-0973-4315-8a68-882792670330" />

**2. Making the jigsaw pattern procedural**

## Features & How They Work
1. Crowd Simulation
   a. Path Finding
   b. Protesting & Dragging
   b. Customers
3. Procedural Artwork/Jigsaw Materials
4. Procedural Jigsaw
5. Day Cycle & End of Day Performance Review

# Original Assets
We created our models with Blender.

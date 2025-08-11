# Arthur's Intelligence Agency

Neha Thumu, Claire Lu

<img width="1181" height="657" alt="image" src="https://github.com/user-attachments/assets/a5226779-6b5e-4b65-8a4e-e86c88e65acb" />

## Game Overview
In this game, you play as an employee of Arthur's Intelligence Agency, a company that creates artworks for various customers. You operate a special machine that absorbs existing art and spits out pieces for you to arrange into new art.

## Major Milestones
**1. Alpha -- Setup, Crowd Sim, Request System**   

   For the alpha version of our game, we created a basic crowd simulation where agents move towards targets. These targets were represented by cubes that were placed at shop entrances. The crowd simulation involved basic seeking and         avoidance behavior, moving towards the specified targets and moving away from other agents in the crowd.

   For the request system, we created reusable dialogue containers to hold the requests of customers and the art that artists created. Our implementation for request system ensures that, during gameplay, artwork that a customer requests     is always able to be created by stealing art from artists -- in other words, artists will always create art that the player would need to use as inputs for the machine.

   Below are some early screenshots of our game in this stage.
   
   <img width="1585" height="664" alt="Screenshot 2025-06-18 195148" src="https://github.com/user-attachments/assets/c2d1ebbd-ce35-4841-894b-42929528f5e3" />

   <img width="643" height="293" alt="Screenshot 2025-06-16 213654" src="https://github.com/user-attachments/assets/a15c3c24-03df-4701-ad0f-ea83b1b877e4" />

**2. Beta -- Jigsaw mechanic, Jigsaw materials, Requests + Jigsaw Integration**
   
   For the beta version of our game, we created a jigsaw puzzle mechanic for the machine operating part of our game loop. We implemented our own puzzle generator that outputted a variety of different puzzle pieces. We wanted each of the     puzzles generated to look visually distinct from each other, so that the game wouldn't feel repetitive.

   Another large part of beta was creating the procedural jigsaw materials. This was an important for creating the outputs of the machine, and helping make the AI metaphor in the game more explicit. For the procedural materials, we          wanted to have basic versions of the materials, which would be used for artists' artwork. On top of this, we wanted to have animated materials that visually looked like a combination of two basic materials. We did this by editing the     Shadergraphs of some Unity provided materials, adding nodes for movement and exposing parameters, like color.

   <img width="464" height="247" alt="Screenshot 2025-07-18 195341" src="https://github.com/user-attachments/assets/cacff9e2-cd6b-4fad-b1a1-6b8daa2dbcb0" />

   We then began integrating the jigsaw logic with our requests logic. This involved some reworking of our code because we originally had dummy sprites to represent the requests. We had assumed we would use sprites rather than materials     for the artwork created by artists and originally structured our code to follow this.

   <img width="1165" height="657" alt="image" src="https://github.com/user-attachments/assets/0121f185-d7d9-4447-a3da-b0ae163e745d" />


**3. Final -- Machine Progression, Sketchbook Event, Day Cycle, Game Logic Polishing**

   At this point, we needed to put all of the parts together, making it feel cohesive with our game logic. We wanted there to be a progression that the machine followed, starting with static images and eventually creating animated images. This was meant to motivate the player to continue creating drawings, having a more expanded skillset as they progressed in the game.

   The sketchbook event is an event that happens when the player has reached a certain point in the game. Up until this point, the player has been creating artwork by using the machine. With our game, one of our goals was to communicate the importance of maintaining human-made things in an increasingly automated/ai generated world. The sketchbook event is supposed to signify a reconnection with human creation. The idea is that the player can choose to create artwork with the sketchbook. Even though it is potentially slower and less refined than a machine-generated piece of art, it contains much more character and meaning than the machine's output. The world, having become grayer with machine use, gradually gets its color back.
   
## Significant Difficulties
**1. Creating "uneven" jigsaw pieces**

Many of the online resources we found on creating jigsaw puzzles focused on square, very gridlike puzzles. The main two resources we looked at are [here](https://youtu.be/9mSwbMiV3lU?si=hLj-cyWk0WKXMJFg) and [here](https://youtu.be/rgm3nityU-M?si=U9nsOdXCqtZmj7hu). The first one implements Bezier curves to create the shape of the puzzle piece's edges. However, the implementation for creating the jigsaw board relies on all edges having the same shape, as it simply uses rotations & reflections of the single Bezier curve. The second resource also creates square pieces and does not cover creating the tabs and indents of a puzzle piece.

These two resources had very different implementations and we didn't anticipate that creating a jigsaw puzzle would be a major time sink for our project. However, after testing the first implementation (and finding the Bezier curves overly complicated for our purposes) we ended up following the second resource as a foundation, and subsequently spending a lot of time figuring out how to implement all the other visual elements we wanted our puzzle to have.

Some challenges along the way:
- Initially, following the second resource, we created the pieces of the puzzle by creating Quad meshes and tiling a material with a test image. By setting the Tiling and Offset of the material to certain values corresponding to the piece's location relative to the puzzle, we were able to set each piece's material to the correct section of the whole image. However, we introduced uneven sized pieces by making each of the columns in a puzzle variable widths (where a column is defined as a vertical stacks of puzzle pieces). Even thought the image would still look seamless, it caused the image to look stretched for bigger columns and squeezed for smaller columns. Thus, rather than adjusting Tiling and Offset of the material, we decided to create the Mesh through code and properly set the UVs.
- However, finding the correct vertex coordinates and setting the UVs was not very simple. Following our reference image from Florence, many of the pieces are trapezoidal. For bottom edge pieces and top edge pieces, it was easy to know what the vertex corners were because the bottom two or top two would always be aligned with x=-0.5 or x=0.5 when visualizing the piece at the origin. Since our implementation defined the heights of the sides of the trapezoid, it was easy to find the other two vertex corners. However, because of the trapezoidal nature of all the pieces, there was no way to tell what the vertex corners were for the middle pieces. We ended up keeping track of the accumulated height of the heights of each side of the columns in order to determine the proper placement of vertex corners for each piece. The vertex corners were converted to UV space by putting them in world space and converting to screen space. This allowed for correct material setup on uneven pieces. Below are some initial attempts.
   <img width="500" height="566" alt="Screenshot 2025-06-27 194254" src="https://github.com/user-attachments/assets/15a2217e-32de-41d0-9021-34543029067e" />

   <img width="500" height="645" alt="Screenshot 2025-06-27 193948" src="https://github.com/user-attachments/assets/cde3a07c-5729-44f5-8325-ad7ba3d6138d" />

   And an example with the correct UVs:

   <img width="500" height="569" alt="Screenshot 2025-06-28 151614" src="https://github.com/user-attachments/assets/937d9f3e-0973-4315-8a68-882792670330" />

**UI in a 3D world**

Something that came up quite often for us is trying to wrangle with the UI without messing up our core game mechanic: the ability to drag the artwork. We found that canvas was a bit clunky to work with in this regard so we changed to having most of our UI be game objects with sprite renderer components and box colliders to facilitate raycasts. However, this led to spending quite a bit of time on finding the proper place to put these attributes on the screen. In order to make sure our UI was properly visible, we also had to edit the sprite order of everything. In the end, we did use the canvas for a few aspects: namely the not interactable components, the sketchbook (which required a bit of finagling to ensure one can not alter the game while sketching), and the event where the user needs to pick between the sketchbook and the wrench. Interestingly enough, the event had some complications with our typical game objects with sprites method for some unknown reason so we ended up using buttons. 

**Missing Sprite Resolution**

We decided to allow the user to submit incorrect inputs for customer requests and treat them as a completed request. However, due to how we created the system of requests and artworks that go along with the requests, we needed to place the incorrect input back in the scene in case a future request may require it. So, we swapped out the incorrect sprite with the correct one. This led to a lot of frankly odd code. 

For example, our artwork has three categories that the sprites can be: color1, procedural, color2. Although color1 and color2 take their colors from the same set, they have different ID's associated with them. From the system side, they are different colors. However, from the user side they look exactly the same. So we had to make sure they are treated the same way. 

The biggest issue was we initially used GameObject.Find to get our correct sprite for the swap which was all good except when you try to find the same object twice in a row. When we replace the sprite we also rename it to have the correct ID. Although visually, it looked like the correct ID, we believe there may have been an issue with the naming-there may have been a hidden character or something of the sort. In order to truly fix this, we had to completely change the way we were finding sprites. We ended up creating a sprite dictionary that kept track of all active sprites and that facilitated the look up and changing of sprites. 

## Features & How They Work
**1. Crowd Simulation**
   a. Path Finding
   b. Protesting & Dragging
   c. Customers

   The crowd simulation involves agents that move towards a destination, defined by (invisible) targets placed in front of shop entrances. There are two types of agents: Artists and Customers. The Customers will approach the machine/player and request a type of art. The Artists will move around the map and periodically create art. Artists have the ability to protest. This means that they will huddle around the machine which makes it more difficult for the player to fulfill requests. An Artist has a higher chance of protesting if the player is further in the game and if they have completed more requests. Artists nearby protesting artists have a chance of joining in on the protest. In order for the player to disperse the protestors, they can simply click and drag a group of protesters away from the machine.
   
**2. Request System**

The request system includes the dialogue bubbles that appear when a customer requests an art piece from the player and the corresponding art that the player would need to take from artists walking around in the scene. The system is implemented so that any art the player would need for a certain request is visible in the scene and is possible to make.

<img width="408" height="252" alt="image" src="https://github.com/user-attachments/assets/bab108cd-84a2-4f96-b4b2-7e62d69ce5d9" />

<img width="415" height="237" alt="image" src="https://github.com/user-attachments/assets/a21ec5a6-4fa8-41a1-893f-f837d38968c4" />

When the player steals from an artist, the artist will react with a negative response.

<img width="1170" height="646" alt="image" src="https://github.com/user-attachments/assets/ce03347f-d048-4715-bcb5-726569662fef" />



**3. Procedural Artwork/Jigsaw Materials**

The artwork created by artists gets inputted into the machine. The output is a combination of the inputs. Customers may request a combination of 1, 2, or 3 art styles. The player searches for art of this style among the artists' drawings. The outputted art will be a combination of the inputs, so that the output may utilize the pattern of the first artwork with the color of the second, for example. This procedural artwork is also animated for an added layer of difficulty. 

**4. Procedural Jigsaw**

The jigsaw puzzle is a core mechanic of the game. Whenever the player fulfills a request, they must create new art by using the machine to generate the pieces of the art and then arrange it. This is the jigsaw puzzle. Because it is repeated many times throughout the game, we wanted our implementation for jigsaw puzzles to be parameterized and able to generate uneven shaped puzzle pieces. This would help introduce variation to the kinds of puzzle the player would encounter. By uneven, we mean that puzzle pieces are not necessarily square, their widths and heights can be different from each other, and the horizontal edges can be diagonal. We also visually wanted the horizontal edges of adjacent puzzle pieces to flow from one to the other, avoiding jutting corners that stick out.

Here's a reference image from the game Florence. 

<img width="200" height="591" alt="image" src="https://github.com/user-attachments/assets/6e3c699b-1cb1-40a5-b62c-7741fbe92fdb" />

Our implementation for the jigsaw puzzles allows for trapezoidal pieces with variable widths and heights. There are no jutting corners, ensured by adjacent sides of two pieces being the same height. The pieces are populated in a pile next to the puzzle board. When the player drags a piece to the correct location on the board, it snaps in place. Z sorting is also implemented so that the most recently clicked pieces are rendered in front. Throughout gameplay, puzzles of different dimensions (rows and columns of the jigsaw) can appear. Tabs and indents appear on every non border edge and are different from puzzle to puzzle (assigned randomly during puzzle initialization). Widths and heights of each piece are also variable for each puzzle.


<img width="500" height="653" alt="image" src="https://github.com/user-attachments/assets/a54b6544-e7dc-424e-84db-a26873c9a675" />

<img width="498" height="235" alt="image" src="https://github.com/user-attachments/assets/11641fec-404f-4aaa-8190-904dcc188229" />

**More examples:**

<img width="500" height="222" alt="image" src="https://github.com/user-attachments/assets/c4acc847-bc67-46bb-85d4-1e177a1787d5" />

<img width="500" height="242" alt="image" src="https://github.com/user-attachments/assets/897f290d-0cff-4f3f-9baf-658aa9602fc8" />

<img width="500" height="241" alt="image" src="https://github.com/user-attachments/assets/96c5b193-038d-43b1-96b3-0101f3a8d7a6" />

<img width="500" height="213" alt="image" src="https://github.com/user-attachments/assets/585d171b-f081-4618-ac92-6a4706e77f24" />

<img width="500" height="240" alt="image" src="https://github.com/user-attachments/assets/0eb5868a-3790-4e64-a404-9eae79b74f9d" />

<img width="500" height="238" alt="image" src="https://github.com/user-attachments/assets/37ffcd49-50f8-4da7-bb8f-dbbf17861d09" />


**5. Day Cycle & End of Day Performance Review**

In order to include more feedback into the game and have the player develop a more emotional reaction from their actions (stealing art from artists) we added the day cycle/end of day performance review. Each day lasts a certain length of time, as indicated by the clock in the upper right hand corner of the game. 

<img width="186" height="87" alt="image" src="https://github.com/user-attachments/assets/0858b2c4-11a2-468e-ab98-b846b2ae3db6" />

The player can see how many days have passed by looking at the calendar. At the end of every day, the player is given a performance review, which tells them how well they did that day and whether their manager is pleased with them or not. This is dependent on the number of correct requests they completed. They are also given a "life update". For example, they can be told that they just bought a car or if they're tight on money and can barely afford rent. Lastly, the player is given a "world update". This update hints to the player the state of the world. It can tell the player that artists are angry or might speak to the increasing grayness of the world. This day cycle feedback is meant to communicate to the player how their choices are affecting them and the world around them.

<img width="500" height="652" alt="image" src="https://github.com/user-attachments/assets/f0e9dbf4-7ed0-414d-b0b2-6a7bb85bdaa2" />

<img width="500" height="647" alt="image" src="https://github.com/user-attachments/assets/a2cc2d6f-2547-435e-bf5a-2034d616e668" />

<img width="1170" height="657" alt="image" src="https://github.com/user-attachments/assets/3b2493f6-9ee4-4f52-83ef-6f6c9e487d74" />

**6. Sketchbook**

If the player chooses to go this route, they can fulfill requests with the sketchbook rather than by fulfilling requests. This option appears midway through the game where the machine breaks and the player is forced to choose between fixing it or not. The sketchbook has several choices for sizes of the brushstroke, the mode (pencil or eraser), and colors. 

(We also have a save/load feature however that did not end up being utilized in the game.)

<img width="1327" height="759" alt="Screenshot 2025-08-10 214047" src="https://github.com/user-attachments/assets/035e9675-52ed-4b34-b2d1-021d049abc1f" />

After the player is done drawing, they can click the green checkmark to submit their drawing to the customer. 

# Future Work 

**1. Gallery of Artwork**

Due to time constraints and a bit of uncertainity about the features we did not utilize our save/load abilities for the artwork created in the Sketchbook. We potentially envisioned saving the user's artworks and having them on display-either as an option within the title screen or potentially within the game itself. This would be interesting to incorporate in our game. 

**2. NPC Immersion**

We really liked Adam's idea of having a chance to chat with the artists that you are impacting with your actions (as the player). As we initially wanted the game to be relatively fast paced, we were not sure where to throw this in but we think it may be an interesting addiion between the days. Given time, we would love to create dialogue for the NPC's and make the action of stealing art be more impactful on the player. 

**3. Beautifying**

Some things we wanted to add to our prototype given time are: making the title screen look more appealing (we were potentially thinking about making it look like the screen of a CRT), adding an explicit counter for the number of requests it takes until the game is over (we had a progress bar but removed this as it was distracting), creating a prettier road, and making the dialog boxes look nicer.

# Original Assets
We created our models with Blender.

The bird asset is also animated in Blender.
<img width="1271" height="828" alt="image" src="https://github.com/user-attachments/assets/8844eb4d-a981-4432-b18a-f63fdf1c1663" />

Here are our environment models! 
<img width="870" height="776" alt="image" src="https://github.com/user-attachments/assets/898d2d43-fbd9-42d6-8a20-14dced248587" />
<img width="914" height="691" alt="image" src="https://github.com/user-attachments/assets/6656beea-b2ad-4c59-a162-088bb7d02b6c" />
<img width="1283" height="1085" alt="image" src="https://github.com/user-attachments/assets/de803a5b-c0e6-43d7-80d9-17443e57dca5" />
<img width="572" height="1234" alt="image" src="https://github.com/user-attachments/assets/119fbb35-171d-49fa-86a8-5501cca1fdf6" />
<img width="901" height="885" alt="image" src="https://github.com/user-attachments/assets/855278ba-3dc7-4ad7-baa3-942271fbed12" />
<img width="788" height="1203" alt="image" src="https://github.com/user-attachments/assets/737cb943-b591-4ef0-a237-8c5c0c42eae6" />
<img width="714" height="995" alt="image" src="https://github.com/user-attachments/assets/6dfe69fe-b1c8-407c-9380-9ce47530cb98" />

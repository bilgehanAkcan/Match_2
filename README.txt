Before starting the game, you can click on the GameCanvas in the Inspector to change the move count, the number of goals to achieve, and the target objects. To change the target object, drag and drop an object from the "Images" folder inside the "Audio&Images" folder in the Assets directory to the relevant field in the Inspector.
Additionally, you can click on the ItemGroup Game Object under GameCanvas and change the number of rows and columns in the Inspector.

!IMPORTANT!: If the values for GameCanvas and ItemGroup Game Object are lost in the Inspector, the SerializeFields need to be initialized as mentioned above. In my working project on my computer, the values are as follows:

ItemGroup:
Rows: 10
Columns: 10
Spacing: 5
Candy Prefabs: [cube_1, cube_2, cube_3, cube_4, cube_5, Balloon, Duck, rocket_left, rocket_right, cube_particle, rocket_up, rocket_bottom] => The elements of the CandyPrefabs array with a length of 12 should be initialized in this way. These elements can be accessed from the Prefabs folder in the Assets directory.

GameCanvas:
Moves Count: 45
Goal 2 Count: 30
Goal 1 Count: 25
Button 2 Sprite: "Green" named image
Button 1 Sprite: "Yellow" named image
Goal 2: Goal2 (Button) => Button under BackgroundCanvas named Goal2
Goal 1: Goal1 (Button) => Button under BackgroundCanvas named Goal1
Moves Text: MovesText => MovesText field under BackgroundCanvas

For the best experience, it is recommended to start the game in 16:9 Portrait mode.

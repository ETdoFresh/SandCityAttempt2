----------------
 Topics
----------------

S01 
	- Sand City (Game name pending/evolving)

	E01
		- Unity Basics
		- Navmesh Agent Movement

	E02
		- RigidBody Movement
		- Animation Movement
		- Putting it all together

	E03
		- Putting it all together
	E03BONUS
		- Add Jump, Crouch, and Walk
	E04
		- Camera Movement
		- Switch from 3rd Person to 1st Person
		- Big Picture Planning
	E05
		- Create a few models
			- Player Model
			- Sand Castle 1, 2, and 3
	E05BONUS
		- Left Shift Walk
		- Camera Depths. Main should be highest.
		- Commit our first git
		- Upload our first remote git
		- Setup cloud build
	E06
		- Show results of cloud build
		- Controls on Camera (View affects controls)
		- Creation of the Mold
	E06BONUS
		- Look at Particles
	E07
		- 3rd Person Camera Controls (WASD to rotate camera) - Partial
		- 1st Person Camera Controls (Movement of camera) - Partial
		- Destroying a Sand Castle - Setup Particle System and a bad mound
	E08
		- Fix Particle appearing weird places (mound was clipping with ground cause particles to shoot everywhere)
	E09
		- Create a beach terrain (Maybe?)
		- Deformation of the Terrain
			- Built-in Unity Terrain (might be too slow for the game)
	E10
		- Collect Sand to make molds
		- 3rd Person Camera Controls (WASD to rotate camera) - Done
	E11
		- 1st Person Camera Controls - Moves player instead of camera
			- Suggestion: 1st Person Camera should act like FPS
		- Merged Terrain Scene and Movement Scene
		- Changed Controls from placing Sand Castles to digging
	E12
		- Optimize/Refactor Code
		- Fix Movement by increasing groundcheckdistance
	E13
		- Add some more sand castles
	E14
		- Change Controls so that it's a PC only game
		- Added scrollwheel for zoom 3rd person
		- Added right click for orbit 3rd person
		- Added Sand Counter GUI
		- Removed Camera Pan 1st Person
	Update E15-E22
	E22
		- Fix Movement - by mutiple ground checks? 4 or 5 maybe?
	E23
		- Add 1st Person RigidBody Movement
	E24
		- Move in 1st Person
	E25
		- Fix nuisances in 1st person
		- Animator stuff (motion not correct)
		- Rotation of camera/player switching between modes
	E26
		- Started working on Top-Down Camera
		- Changed some materials and added "water"
	E27
		- Finish Top-Down Camera
	E37
		- Foot Step Script - Place at rate walking, don't place on standing still
	E38
		- Controls - Worked on Scoop.
	E39
		- Created a raycast input ground object.
	FUTURE
		- Controls - Place Sand Castle Mode
		- Controls - Smooth Terrain Mode
		- Controls - Continuous action, like scoop or finger.
		- Controls - Dig/Place Sand (In place now)
		- Water waves
		- Option Screen for Controls and Resolution and other stuff
		- Menu Screen
		- Sandy Beach Life (birds)
		- Marine Life
	LOW PRIORITY
		- Slow Radius on Movement
		- Transition from 1st Person Cam to 3rd person (instead of cutting between cameras)
		- Networkability (Server/Client)
		- Sharks? Water Spouts? Pirates?
		- Fix Groundcheck Gizmo, draw only on selected
	BUG REPORT
		- Animation stops when player is stuck…. Not really sure this is a good thing
		- Crouch Walk Speed doesn't match Movement Speed
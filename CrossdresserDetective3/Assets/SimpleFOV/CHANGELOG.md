# Change Log

## [1.0.0] - 2024-10-1

The tool is ready for release.

## [1.1.0] - 2024-12-29

Based on feedbacks, change how the object detection for **3D** FOV is exposed. (2D is not affected in this update.) Now object detection is simpler and less implementation is needed from the user side.

#### Change list for FOV3D:

- Removed *IDetectBlocker* and *IDetectNonBlocker*

- Detection information is available through properties or unity events now. Set up is in inspector instead of code now. 

- Demo scenes and scripts were updated according to the changes. See "ExampleBot - with detection" in the demo scene to see how the detection is done. Documentation was also updated.

## [1.1.1] - 2025-01-01

Fixed a previous bug that the detection reported multiple entries for the same detected object. Add optional editor visualization for object detection for easier debugging.

## [1.1.2] - 2026-03-02

Fixed a reported bug that the field of view is incorrectly displaying the fov area and incorrectly extending itself when encontering obstacles, which only happens if the fov or its parent is not the default scale.

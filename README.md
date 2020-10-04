# VisualPSO

![Image](https://github.com/DiogoDeAndrade/VisualPSO/raw/master/Screenshots/title.png)

## Summary

Visual PSO is an application that allows for the visualization of
the PSO algorithm, displaying both the objective function and the particles.

It supports a wide range of functions, and several visualization modes.

## Synopsis

The application can be run as an executable with command line parameters to customize
the visualization, or it can be used as an Unity project, allowing a better interface for
configuration.

## Command line interface

If the application is ran without parameters, a random visualization will be generated from
all available options.

### Presets options

Parameter      | Description
-------------- | -----------
-preset\<number>| Selects one of the builtin presets. Number must be an integer in the range [0, 13]
-experiment\<number>| Selects one of the builtin experiments, as represented in this article. Number must be na integer in the range [1,10].
-random | Generates a random visualization. Same as not passing any parameter. 

### Function selection options
Parameter      | Description
-------------- | -----------
-landscape | Generates a Perlin based landscape function. The Perlin noise PCG options can be used to further customize the function
-imagesaturation| Uses an image's HSV Saturation as a function. The image options can be used to further customize the function
-imagevalue | Uses an image's HSV Value (brightness) as a function. The image options can be used to further customize the function
-ralphmean | Uses Ralph's bell curve mean as function. The image options can be used to further customize the function
-ralphvar | Uses Ralph's bell curve variance as function. The image options can be used to further customize the 
-sphere | Use the sphere function.
-quadric | Use the quadric function.
-hyperellipsoid | Use the hyperellipsoid function.
-rastrigin | Use the Rastrigin function.
-griewank | Use the Griewank function.
-schaffer | Use the Schaffer function.
-ackley | Use the Ackley function.
-weierstrass | Use the Weierstrass function.

### Perlin noise PCG options                     

Parameter      | Description
-------------- | -----------
-octaves\<number> | Numbers of octaves for the Perlin landscape. Default is 8.
-amplitude\<value> | Initial amplitude for the Perlin landscape. Default is 20.
-frequency\<value> | Initial frequency for the Perlin landscape. Default is 0.04.

### Image options

Parameter              | Description
---------------------- | -----------
-sampleradius\<radius> | Defines the radius to use as the neighborhood for computing Ralph's bell curve. Note that this is a O(n²)operation, so large radius will take some time to compute. Default value is 5.
-useresponse | Use the response value for computing Ralph's bell curve. This is the default.
-usestimulus | Use the stimulus value, instead of the more traditional response value for computing Ralph's bell curve.
-image\<index> | Selects a predefined image as the source image. Index must be na integer in the range [0,3].
-image\<filename> | Loads an image from the given path and uses it as source image. Only JPG and PNG are valid.

### PSO options
Parameter              | Description
---------------------- | -----------
-w\<value> | Specifies ω. Default is 0.7.
-c\<value> | Specifies C<sub>p</sub> and C<sub>g</sub> with the same value. Default is 1
-c1\<value> | Specifies the C<sub>p</sub> value. Default is 1
-c2\<value> | Specifies the C<sub>g</sub> value. Default is 1
-vmax\<value> | Specifies the maximum speed for the particles. Default is 1

### Visual options
Parameter              | Description
---------------------- | -----------
-scale\<value> | Allows to scale the Y values of function. Default is 1.
-material\<index> | Selects the material to use for the visualization. Index must be an integer in the range [0,5]. Default is 0.
-fof | Enables the fog of function option.
-connectivity | Displays the particle connectivity.

### General options
Parameter              | Description
---------------------- | -----------
-speed\<number> | Defines the speed of the simulation. Default is 1, 2 is twice the speed, 0.5 is half-speed.
-rngseed\<number> | Seed for random number generator.

### Usage examples

```
.\VisualPSO -experiment5
.\VisualPSO -preset4 -speed2
.\VisualPSO -landscape -octaves4 -amplitude10 -frequency0.01
.\VisualPSO -imagevalue -image"d:\some path\test.jpg" -material3
.\VisualPSO -ralphmean -image2 -sampleradius3 -usestimulus
.\VisualPSO -rastrigin
```

## Unity project

To use the Unity project, the scene used for the executable is ExperimentScene.

Under Prefabs, there is a sample prefab that can be used for testing called TestVisualPSO. It can drag be dropped in the scene and
adjust the parameters of the PSO and the visual display on the instance.

To configure the PSO itself, you can modify the parameters on the PSOConfig behaviour:

![Image](https://github.com/DiogoDeAndrade/VisualPSO/raw/master/Screenshots/unity_pso_config.png)

Most parameters are identical to their command line counterparts, except for the following:

Parameter              | Description
---------------------- | -----------
Function Sampling Size | Number of points sampled for the terrain. Default is 257. Changing this value might require play to be triggered twice
Seed | This is the seed for the PSO part only. The camera seed must be set on the PSOCameraController on the Main Camera
Algorithm | Selection of the problem to be optimized (Rastrigin, Perlin landscape, etc)
Perlin Amplitude Per Octave | Multiplier for the amplitude for each octave of Perlin noise
Perlin Frequency Per Octave | Multiplier for the frequency for each octave of Perlin noise
Perlin Offset | Specify an offset for the landscape function
XMax | Imposes a limit to the particles movement (on both X and Z axis)
Initial X | Specifies the range for the generation of particles

Visual parameters can be configured on the PSORender behavior:

![Image](https://github.com/DiogoDeAndrade/VisualPSO/raw/master/Screenshots/unity_render_params.png)

Parameter              | Description
---------------------- | -----------
Particle Prefab | Prefab to be used for each particle
Function Prefab | Prefab to use to render the function
Material Override | Material to use to render the function
Color Particles | Color to use for the particles. If "Color By Gradient" is active in the particle prefab, this parameter has no purpose
Time per iteration | Time between PSO iterations. All particle properties are interpolated from the previous to the new position on each iteration
Fog of Function | Enables/Disables the fog of function
Y Scale | Specifies a vertical scale for the function and particles
Particle Scale | Specifies a scale for the particles themselves
Trail Scale | Allows to specify how large the trail of the particle can become
Display Connectivity | Enables/Disables the display of the particle connectivity
Play Speed | Allows to specify the simulation speed, can be changed during runtime

If the CommandLineProcessor object is active on play, the command line is going to be parsed and two simulation will be ran at the same
time, so it should be disabled unless a build that requires command line processing is being performed.

Camera behaviour is controlled through the PSOCameraBehaviour's on the Main Camera object. Control is done on the PSOCameraController behavior.

## Reproducible behavior generation

All behaviour is reproducible, provided the random seed is the first parameter provided.

```
.\VisualPSO -rngseed100 <other parameters>
```
<!-- ## Reference

If you use this function in your work, please cite the following reference:
-->

## Acknowledgements

[OpenPSO.NET]

[Naughty Attributes] by Denis Rizov ([MIT_LICENSE])

## License

[Mozilla Public License 2.0](LICENSE)

![Image](https://github.com/DiogoDeAndrade/VisualPSO/raw/master/Screenshots/screen01.png)

![Image](https://github.com/DiogoDeAndrade/VisualPSO/raw/master/Screenshots/screen02.png)


[Naughty Attributes]:https://github.com/dbrizov/NaughtyAttributes
[OpenPSO.NET]:https://github.com/fakenmc/openpso.net
[MIT_LICENSE]:(MIT_LICENSE)
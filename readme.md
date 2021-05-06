# TODO Verify readme after class renaming!

# Animator

Animator is a command-line utility, which renders animations described in XML files to a series of frames, which later can be joined into a video or embedded in one. The software is meant mainly for power users and developers, but since there are no specific skills required to use this tool, everyone can learn the animation file definition and use the tool.

# Quickstart

Create a text file with following contents:

```xml
<Movie>
  <Movie.Config>
    <MovieConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Rectangle Name="Rectangle" Width="20" Height="20" Origin="10;10" Position="40;50" Brush="Red" />
        
    <Scene.Animations>
        <AnimatePoint PropertyRef="Rectangle.Position" From="40;50" To="60;50" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
        <AnimateFloat PropertyRef="Rectangle.Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
    </Scene.Animations>
  </Scene>
</Movie>
```

Then, execute Animator with the following parameters:

```
Animator.exe render -s D:\Animation.axml -o D:\frame.png
```

This will result in creating 90 frames with names D:\frame0.png, D:\frame1.png, ..., D:\frame89.png containing red square, which rotates and moves from left to right.

# Basics

Animation is described in a custom XML file. Descriptions of all possible tags and their attributes can be found in Animator.html file available in this repository. Note, that you may also quite easily generate this file by yourself with help of Animator.Documentation project. Feed it with Animator.Engine.xml file generated during its build (you may need to adjust target path for that file).

If you are familiar with WPF/Avalonia, you will find some concepts familiar, because the data structure behind animation file definition is built on similar principles.

The root node of the animation file is `Movie`, which contains two attributes: `Config` and `Scenes`. The first one allows you to configure the animation (eg. dimensions of a frame, frames per second etc.), and the second allows you to prepare a list of scenes, which later will be turned into animation frames. Your animation should have at least one.

Each scene consists of `Items`, which is a list of `Visual`s - objects, which may be rendered. Every object in the animation may contain one or more .Animations` - list of objects deriving from `Animation`, which allows you to animate numeric properties of objects in time.

Usually, you have to specify, which property is animated. All animator objects have property named `PropertyRef`, which should contain reference to a property, which will be animated. Such reference consists of series of identifiers separated by dots (.). Each identifier may either point to a property or a child, named object. Root of search is always object, on which you attach.Animations. Check the following example (especially `PropertyRef` values and placement of `FloatAnimation`s.

```xml
<Movie>
  <Movie.Config>
    <MovieConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Rectangle Name="Rectangle" Width="20" Height="20" Origin="10;10" Position="20;50" Brush="Red">
      <Rectangle.Pen>
        <Pen Color="Black" Width="1" />
      </Rectangle.Pen>
    </Rectangle>
    <Rectangle Name="Rectangle2" Width="20" Height="20" Origin="10;10" Position="50;50" Brush="Red">
      <Rectangle.Pen>
        <Pen Name="MyPen" Color="Black" Width="1" />
      </Rectangle.Pen>
    </Rectangle>
    <Rectangle Name="Rectangle3" Width="20" Height="20" Origin="10;10" Position="80;50" Brush="Red">
      <Rectangle.Pen>
        <Pen Name="MyPen" Color="Black" Width="1" />
      </Rectangle.Pen>
      <Rectangle.Animations>
        <AnimateFloat PropertyRef="MyPen.Width" From="0" To="10" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="Linear" />
      </Rectangle.Animations>
    </Rectangle>
        
    <Scene.Animations>
        <AnimateFloat PropertyRef="Rectangle.Pen.Width" From="0" To="10" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="Linear" />
        <AnimateFloat PropertyRef="Rectangle2.MyPen.Width" From="0" To="10" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="Linear" />
    </Scene.Animations>
  </Scene>
</Movie>
```

## Visual

Visual is an object, which may be rendered on the scene. Every visual have a set of basic properties: `Origin`, `Position`, `Rotation`, `Scale`, `Alpha`, `Mask`, `InvertMask`, `MaskCoordinateSystem` and `Effects`.

Every visual is drawn in its own coordinate system - so for instance, if you draw a rectangle, its left-top corner will always have coordinates of (0, 0). To position it properly in the scene, you should first define its `Origin` - point in local coordinates, which will act as an anchor. When you later set `Position` of the `Visual`, Animator will move it in such way, that its `Origin` reaches desired point. `Origin` also serves as center of rotation and axis for scale.

`Mask` allows you to selectively hide or show parts of the `Visual`. Use `Visual`s to draw a mask, just keep in mind, that only alpha value of mask pixels will be later taken into consideration. By default, pixels with alpha value 1 will be rendered, pixels with value 0 will not be rendered, and values in-between will effect in semi-transparence. You may set property `InvertMask` to true to define areas, which should *not* be drawn - the described behavior will be inverted. Also, you may use `MaskCoordinateSystem` property to specify, in which coordinate system do you want to draw the mask. If you choose `Local`, then mask will follow all transforms applied to a `Visual` and if you choose `Global`, then it will be immune to them. The following example shows, how to use the mask and explains difference between two coordinate systems.

```xml
<Movie>
  <Movie.Config>
    <MovieConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Rectangle Name="Rectangle" Width="20" Height="20" Origin="10;10" Position="40;30" Brush="Red" InvertMask="True" MaskCoordinateSystem="Local">
      <Rectangle.Mask>
        <Circle Center="20;10" Radius="10" Brush="#ff000000" />
      </Rectangle.Mask>
    </Rectangle>

    <Rectangle Name="Rectangle2" Width="20" Height="20" Origin="10;10" Position="40;70" Brush="Red" InvertMask="True" MaskCoordinateSystem="Global">
      <Rectangle.Mask>
        <Circle Center="50;70" Radius="10" Brush="#ff000000" />
      </Rectangle.Mask>
    </Rectangle>

    <Scene.Animations>
        <AnimatePoint PropertyRef="Rectangle.Position" From="40;30" To="60;30" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
        <AnimateFloat PropertyRef="Rectangle.Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />

        <AnimatePoint PropertyRef="Rectangle2.Position" From="40;70" To="60;70" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
        <AnimateFloat PropertyRef="Rectangle2.Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
    </Scene.Animations>
  </Scene>
</Movie>
```

Currently the following `Visual`s are available:

* Rectangle
* Circle
* Ellipse
* Path (its definition is mostly SVG-compatible)
* Line
* Label (text)

## Effects

You can add effects to a visual - at the time of writing, there are three different effects: `Blur`, `GaussianBlur` and `DropShadow`. Effects are applied after rendering, but before applying mask. If you want to apply effect after applying mask, place the `Visual` on a `Layer` and then add an effect to the `Layer` instead. The following example shows difference between those two scenarios.

```xml
<Movie>
  <Movie.Config>
    <MovieConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Rectangle Name="Rectangle" Width="20" Height="20" Origin="10;10" Position="40;30" Brush="Red" InvertMask="True" MaskCoordinateSystem="Local">
      <Rectangle.Mask>
        <Path Definition="M 21 -1 L -1 21 H 21 V -1 Z" Brush="#ff000000" />
      </Rectangle.Mask>
      <Rectangle.Effects>
        <DropShadowEffect Color="#a0000000" DX="5" DY="5" Radius="5" />
      </Rectangle.Effects>
    </Rectangle>

    <Layer Name="Layer" Origin="10;10" Position="40;70">
      <Rectangle Width="20" Height="20" Brush="Red" InvertMask="True" MaskCoordinateSystem="Local">
        <Rectangle.Mask>
        <Path Definition="M 21 -1 L -1 21 H 21 V -1 Z" Brush="#ff000000" />
        </Rectangle.Mask>
      </Rectangle>
      <Layer.Effects>
        <DropShadowEffect Color="#a0000000" DX="5" DY="5" Radius="5" />
      </Layer.Effects>
    </Layer>

    <Scene.Animations>
      <AnimatePoint PropertyRef="Rectangle.Position" From="40;30" To="60;30" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
      <AnimateFloat PropertyRef="Rectangle.Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />

      <AnimatePoint PropertyRef="Layer.Position" From="40;70" To="60;70" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
      <AnimateFloat PropertyRef="Layer.Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
    </Scene.Animations>
  </Scene>
</Movie>
```

## Property animations

Almost every numeric field (float, int or PointF) may be animated. The simplest way to do so is to use `FloatAnimation`, `IntAnimation` or `PointAnimation`. All of those expose properties `From` and `To`, defining border values of the animation, `StartTime` and `EndTime` to denote the time period, when property is animated and `EasingFunction`, which defines way in which value is changed.

Downside of `Animation`s is that they are meant to be used only once per scene per property. If you use two, value of the last one will remain. If you want to design multiple animations for a property, use `Storyboard` instead.

You have to explicitly say, which property is animated. For that you use `PropertyRef` to specify object and its property to animate. You should do it with familiar dot-as-member-access notation, such as `Rectangle.Pen.Width`. You may walk through object's properties and child items (by child items we understand those kept in reference properties and collections).

`PropertyRef` will fail to find property if:

* None of properties or child items match given identifier, or
* Multiple child items match given identifier, or
* Both property and child match given identifier, or
* Object on the path does not derive from `BaseElement` (you cannot reach simple, CLR properties, only managed ones)

To make a visual reachable for the.Animations, give itself a `Name`. Names should be unique. No immediate checks are performed, if names are unique, but if you try to find object with non-unique name, animation will fail with an error.

You can quickly see, that often a lot of values of various properties of `Animation`s are shared (for instance, in the previous example, all `Animation`s share values of `StartTime`, `EndTime` and `EasingFunction`. To simplify notation, you may group animations with `AnimationGroup` and take advantage of property inheritance to shorten the code:

```xml
<Scene.Animations>
  <AnimationGroup StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth">
    <AnimatePoint PropertyRef="Rectangle.Position" From="40;30" To="60;30" />
    <AnimateFloat PropertyRef="Rectangle.Rotation" From="0" To="360" />

    <AnimatePoint PropertyRef="Layer.Position" From="40;70" To="60;70" />
    <AnimateFloat PropertyRef="Layer.Rotation" From="0" To="360" />
  </AnimationGroup>
</Scene.Animations>
```

There's one `Animation`, which may be used in advanced scenarios, namely `AnimateWithExpression`. You may define mathematical expression, which will be evaluated and put into specified property. The following example should explain, how does it work.

```xml
<Movie>
  <Movie.Config>
    <MovieConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Rectangle Name="Rectangle" Width="20" Height="20" Origin="10;10" Position="40;30" Brush="Red" />
    <Rectangle Name="Rectangle2" Width="20" Height="20" Origin="10;10" Position="40;30" Brush="Green" />
      
    <Scene.Animations>
      <AnimatePoint PropertyRef="Rectangle.Position" From="40;30" To="60;30" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />
      <AnimateFloat PropertyRef="Rectangle.Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="CubicBoth" />

      <AnimateWithExpression PropertyRef="Rectangle2.Position" Expression="Rectangle.Position + [10,10]" /> 
    </Scene.Animations>
  </Scene>
</Movie>
```

## Storyboard

Storyboard allows you to define multiple keyframes with multiple easing functions for multiple properties. Each keyframe specifies a value at a time and easing function. The easing function is applied to segment *before* current keyframe. If there is none, one is assumed at time 0 with initial value of animated property. You can use `Storyboard` in the same way as `AnimationGroup` - to specify some values, which will be shared between all keyframes (if not overwritten explicitly).

You may specify multiple storyboards for a scene.

```xml
<Movie xmlns:e="https://spooksoft.pl/animator">
  <Movie.Config>
    <MovieConfig Width="100" Height="100"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Layer>
        <Rectangle Name="Red" Width="20" Height="20" Origin="10;10" Brush="Red" />
        <Rectangle Name="Green" Width="20" Height="20" Origin="10;10" Brush="Green" />

      <Layer.Animations>
        <Storyboard EasingFunction="QuadSlowDown">
          <PointKeyframe PropertyRef="Red.Position" Time="0:0:0.0" Value="10;10" />
          <PointKeyframe PropertyRef="Red.Position" Time="0:0:1.0" Value="10;50" />
          <PointKeyframe PropertyRef="Red.Position" Time="0:0:2.0" Value="50;50" />
          <PointKeyframe PropertyRef="Red.Position" Time="0:0:3.0" Value="50;10" />
                     
          <PointKeyframe PropertyRef="Green.Position" Time="0:0:0.0" Value="10;10" />
          <PointKeyframe PropertyRef="Green.Position" Time="0:0:1.0" Value="50;10" />
          <PointKeyframe PropertyRef="Green.Position" Time="0:0:2.0" Value="50;50" />
          <PointKeyframe PropertyRef="Green.Position" Time="0:0:3.0" Value="10;50" />
        </Storyboard>
      </Layer.Animations>
    </Layer>
  </Scene>
</Movie>
```

Keep in mind, that keyframes are grouped by their `PropertyRef`s. If you find a way to reach an object in two ways (ie. by property and by its name), such keyframes will be treated separately (most likely with unpredictable results).

## Reusing parts of animation

You may use good-old include mechanism to reuse parts of the animation. To include a file, you have to define additional XML namespace first, and then use `Include` tag.

```xml
Animation.axml:

<Movie xmlns:e="https://spooksoft.pl/animator">
  <Movie.Config>
    <MovieConfig Width="100" Height="100"/>
  </Movie.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Layer>
        <e:Include Source="D:\Rectangle.axml" />

      <Layer.Animations>
          <Storyboard PropertyRef="Red.Position" EasingFunction="QuadSlowDown">
              <PointKeyframe Time="0:0:0.0" Value="10;10" />
              <PointKeyframe Time="0:0:1.0" Value="10;50" />
              <PointKeyframe Time="0:0:2.0" Value="50;50" />
              <PointKeyframe Time="0:0:3.0" Value="50;10" />
          </Storyboard>
      </Layer.Animations>
    </Layer>
  </Scene>
</Movie>

Rectangle.axml:

<Rectangle Name="Red" Position="30;30" Origin="10;10" Width="20" Height="20" Brush="Red" />
```

If you don't specify the absolute path for include file, the current path will be used.
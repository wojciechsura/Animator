# Animator

Animator is a command-line utility, which renders animations described in XML files to a series of frames, which later can be joined into a video or embedded in one. The software is meant mainly for power users and developers, but since there are no specific skills required to use this tool, everyone can learn the animation file definition and use the tool.

# Quickstart

Create a text file with following contents:

```xml
<Animation>
  <Animation.Config>
    <AnimationConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Animation.Config>
  <Scene Name="Scene1" Duration="0:0:3.0" Background="White">
    <Rectangle Name="Rectangle" Width="20" Height="20" Origin="10;10" Position="40;50" Brush="Red" />
        
    <Scene.Animators>
        <PointPropertyAnimator TargetName="Rectangle" Path="Position" From="40;50" To="60;50" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
        <FloatPropertyAnimator TargetName="Rectangle" Path="Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
    </Scene.Animators>
  </Scene>
</Animation>
```

Then, ececute Animator with the following parameters:

```
Animator.exe render -s D:\Animation.axml -o D:\frame.png
```

This will result in creating 90 frames with names D:\frame0.png, D:\frame1.png, ..., D:\frame89.png containing red square, which rotates and moves from left to right.

# Basics

Animation is described in a custom XML file. Descriptions of all possible tags and their attributes can be found in Animator.html file available in this repository. Note, that you may also quite easily generate this file by yourself with help of Animator.Documentation project. Feed it with Animator.Engine.xml file generated during its build (you may need to adjust target path for that file).

If you are familiar with WPF/Avalonia, you will find some concepts, which are similar, because the data structure behind animation file definition is built on similar principles.

The root node of the animation file is `Animation`, which contains two attributes: `Config` and `Scenes`. The first one allows you to configure the animation (eg. dimensions of a frame, frames per second etc.), and the second allows you to prepare a list of scenes, which later will be turned into animation frames. Your animation should have at least one.

Each scene consists of `Items`, which is a list of `Visual`s - objects, which may be rendered - and `Animators` - list of `BaseAnimator`s, which allow you to animate numeric properties of visuals in time. In general majority of float, int and PointF-typed properties are animatable, only with some minor exceptions.

## Visual

Visual is an object, which may be rendered on the scene. Every visual have a set of basic properties: `Origin`, `Position`, `Rotation`, `Scale`, `Alpha`, `Mask`, `InvertMask`, `MaskCoordinateSystem` and `Effects`.

Every visual is drawn in its own coordinate system - so for instance, if you draw a rectangle, its left-top corner will always have coordinates of (0, 0). To position it properly in the scene, you should first define its `Origin` - point in local coordinates, which will act as an anchor. When you later set `Position` of the `Visual`, Animator will move it in such way, that its `Origin` reaches desired point. `Origin` also serves as center of rotation and axis for scale.

`Mask` allows you to selectively hide or show parts of the `Visual`. Use `Visual`s to draw a mask, just keep in mind, that only alpha value of mask pixels will be later taken into consideration. By default, pixels with alpha value 1 will be rendered, pixels with value 0 will not be rendered, and values in-between will effect in semi-transparence. You may set property `InvertMask` to true to define areas, which should *not* be drawn - the described behavior will be inverted. Also, you may use `MaskCoordinateSystem` property to specify, in which coordinate system do you want to draw the mask. If you choose `Local`, then mask will follow all transforms applied to a `Visual` and if you choose `Global`, then it will be immune to them. The following example shows, how to use the mask and explains difference between two coordinate systems.

```xml
<Animation>
  <Animation.Config>
    <AnimationConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Animation.Config>
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

    <Scene.Animators>
        <PointPropertyAnimator TargetName="Rectangle" Path="Position" From="40;30" To="60;30" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
        <FloatPropertyAnimator TargetName="Rectangle" Path="Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />

        <PointPropertyAnimator TargetName="Rectangle2" Path="Position" From="40;70" To="60;70" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
        <FloatPropertyAnimator TargetName="Rectangle2" Path="Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
    </Scene.Animators>
  </Scene>
</Animation>
```

## Effects

You can add effects to a visual - at the time of writing, there are three different effects: `Blur`, `GaussianBlur` and `DropShadow`. Effects are applied after rendering, but before applying mask. If you want to apply effect after applying mask, place the `Visual` on a `Layer` and then add an effect to the `Layer` instead. The following example shows difference between those two scenarios.

```xml
<Animation>
  <Animation.Config>
    <AnimationConfig Width="100" Height="100" FramesPerSecond="30"/>
  </Animation.Config>
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

    <Scene.Animators>
        <PointPropertyAnimator TargetName="Rectangle" Path="Position" From="40;30" To="60;30" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
        <FloatPropertyAnimator TargetName="Rectangle" Path="Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />

        <PointPropertyAnimator TargetName="Layer" Path="Position" From="40;70" To="60;70" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
        <FloatPropertyAnimator TargetName="Layer" Path="Rotation" From="0" To="360" StartTime="0:0:0.0" EndTime="0:0:3.0" EasingFunction="EaseCubicBoth" />
    </Scene.Animators>
  </Scene>
</Animation>
```

## Property animations

Every 
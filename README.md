# Feedforward in Virtual Reality

## Intro
This repo contains the implementation for VR feedforward and three demos showcasing practical applications of feedforward. The implementation is made in Unity for Oculus Quest 1&2.

Folder builds\ contains 2 apks which can be installed on the Oculus headset to load a demo application of feedforward. The demo shows users how to start a car in VR using feedforward. One apk includes interrupts, which means the user can stop feedforward once they touch the previewing objects. 
 
 Please use SideQuest to install the apk’s on Oculus. While starting the applications, please look straight ahead as the demos are calibrated to your height. If there is a problem with calibration, load the Unity project and adjust the values in

To make the implementation open access, feedforward lens has been removed.

## Requirements

Unity 2019.3.15f1

HPTK 0.5.0 [https://github.com/jorgejgnz/HPTK/wiki/Getting-started](https://github.com/jorgejgnz/HPTK/wiki/Getting-started)

Oculus XR plugin 1.3.4 [https://docs.unity.cn/Packages/com.unity.xr.oculus@0.8/manual/index.html](https://docs.unity.cn/Packages/com.unity.xr.oculus@0.8/manual/index.html)

XR Plugin Management 3.2.15

Text Mesh Pro 2.0.1

## Scene descriptions

In Assets\Scenes you can find the demo applications.

1. **Car demo**

The car demo shows the user how to start a car. There are 2 triggers on the wheel, one for the left hand and one for the right hand. The left-hand trigger shows the user how to put the car in gear and press the ignition button. The right-hand trigger shows the user how to turn the wheel. The car does not move, but the motor makes a noise upon ignition. The purpose of this demo is to showcase how feedforward can help users to understand multi-step interactions.  In addition, the virtual car differs from real-cars and the ignition method is different. The user triggers the car when their hands are close to the wheel. 

2. **Car demo travel**

This example showcases how feedforward can be triggered from afar. It is different from CarDemo because preview gets triggered from a bigger distance.  When the preview is triggered, the user is taken to the feedforward location. After the previewing ends, the user is restored to their initial position. Only works with one feedforward at a time.

3. **Car demo with offset**

In this car demo, we showcase how the ghost can be offsetted from their original position, so they do not overlap with the original objects.

4. **Kitchen demo**

In this demo, the user may gaze around a virtual kitchen and see available interactions using feedforward. In this demo, we introduce gaze triggering and show how many previews can be displayed at the same time. The kitchen contains several interactable objects and ghost hands show the users how to manipulate the objects.

5. **Norman door demo**

In this demo we showcase how feedforward can help users interact with objects that have misleading signifiers or affordances. We implemented for this purpose a door which is opened by sliding the handle to the right and pooling the door. These door are called Norman Doors and they have poor affordance (see [https://uxdesign.cc/intro-to-ux-the-norman-door-61f8120b6086](https://uxdesign.cc/intro-to-ux-the-norman-door-61f8120b6086) for more details about bad design in the real world). The purpose of the feedforward is to prevent the user from struggling with the door by setting their expectations of how to interact with it. The door signals to the user that it is opened by pulling, whereas the actual opening mechanism is different.

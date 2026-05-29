<p align="center">
  <img src="https://raw.githubusercontent.com/AtomicTyler1/AtomicTyler1/main/banner.png" width="80%">
</p>
<h1 align="center">Custom TipTap Videos</h1>

<p align="center">Here are my links, it would mean a lot to me if you go a head and check them out :3</p>

<p align="center">
  <a href="https://atomictyler.dev/#projects">
    <img src="https://img.shields.io/badge/Atomic's%20Mods-orange?style=for-the-badge" alt="Atomic's Mods">
  </a>
  <a href="https://modding-community.com">
    <img src="https://img.shields.io/badge/modding--community.com-blue?style=for-the-badge" alt="Modding Community">
  </a>
  <a href="https://discord.atomictyler.dev">
    <img src="https://img.shields.io/badge/Join%20my%20Discord-7289DA?style=for-the-badge&logo=discord&logoColor=white" alt="Join my Discord">
  </a>
  <a href="https://atomictyler.dev/">
    <img src="https://img.shields.io/badge/My%20Portfolio-black?style=for-the-badge&logo=codecrafters&logoColor=white" alt="My Portfolio">
  </a>
  <a href="https://github.com/AtomicTyler1">
    <img src="https://img.shields.io/badge/My%20GitHub%20Profile-black?style=for-the-badge&logo=github&logoColor=white" alt="My Github Profile">
  </a>
</p>
<h1 align="center"></h1>

## What is `Custom Cosmetics`?

This mod is a library, it by default comes with no custom cosmetics.

You **must** have the exact same cosmetics as another person otherwise you or they might we wearing the wrong thing!!

# Guide to creating custom cosmetics

This tutorial will walk you through the step-by-step process of creating and exporting your own custom cosmetics for the game using Blender and Unity.

---

## Prerequisites & Installation

### Step 1: Install the Mod
Before starting, ensure you have the base mod installed. You can download it using your preferred mod manager (like Thunderstore Mod Manager or r2modman) or install it manually.

### Step 1.1: Download the Blender Template
To ensure your custom 3D model fits perfectly, it is highly recommended to build it around the official reference template.
* Download the template here: https://github.com/AtomicTyler1/Custom-Cosmetics/Crashout-Crew.blend
* Make sure not to export the model if you use this!!!

### Step 2: Download Unity Hub
Unity Hub is required to manage your Unity versions and projects. If you don't have it, download and install it from the official Unity website.

### Step 3: Install Unity 2022.3.10f1
Open Unity Hub, navigate to the **Installs** tab, and install version **2022.3.10f1**. *Note: Using the exact version is crucial to avoid compatibility and building errors.*

### Step 4: Install Git
The Unity package manager requires Git to fetch the toolkit. Ensure Git is installed on your system. If not, download it from https://git-scm.com/.

---

## Setting Up Your Unity Project

### Step 5: Create a New Project
1. Open Unity Hub.
2. Click **New Project**.
3. Select the **URP (Universal Render Pipeline)** template.
4. Name your project anything you like and click **Create project**.

### Step 6: Install the Custom Cosmetics Package
Once your project opens, you need to import the official toolkit via Git URL:
1. In the top menu, go to **Window** > **Package Manager**.
2. Click the **+** (plus) icon in the top-left corner of the Package Manager window.
3. Select **Add package from git URL...**
4. Paste the following URL and click **Add**: `https://github.com/AtomicTyler1/Custom-Cosmetics.git?path=CustomCosmetics`

---

## Importing and Preparing Assets

### Step 7: Import Your Model and Textures
Drag and drop your custom 3D model file (e.g., `.fbx` or `.obj`) and its corresponding texture files directly into the **Project** window (Assets folder) inside Unity.

### Step 8: Preview in the Scene
Drag your model from the Project window into the **SampleScene** hierarchy. This allows you to verify that the model, textures, and materials are rendering exactly how you want them to look.

---

## Configuring the Cosmetic

### Step 9: Create a Cosmetic Definition
1. Right-click anywhere in your **Project** window (Assets folder).
2. Navigate to **Create** > **Custom Cosmetics** > **Cosmetic Definition**.

### Step 10: Name the Asset
Give the newly created Cosmetic Definition file a recognizable name.

### Step 11: Set the Cosmetic ID
Select the Cosmetic Definition asset and look at the **Inspector** window on the right:
* Locate the **Cosmetic ID** field.
* Enter a unique file and internal identifier (e.g., `atomic-mushroom`). Use lowercase letters and hyphens for best practice.

### Step 12: Assign Your 3D Models
In the Inspector, you will see slots for **Head**, **Body**, and **Face**:
* Drag your model from the Project asset folder into the slot corresponding to where it belongs on the character.
* *Note: You do not need to fill all three slots. Only use multiple slots if you are creating a complex, multi-part cosmetic.*

---

## Building and Testing In-Game

### Step 13: Build the Cosmetic
With your Cosmetic Definition asset selected, click the **Build Cosmetic** button in the Inspector. This will compile your asset and output the final file, typically to your desktop.

### Step 14: Install the Custom Cosmetic
Locate the generated cosmetic file on your desktop and drag it into your game's **BepInEx/plugins** folder.

### Step 15: Enable Developer Tools
To easily adjust the cosmetic position in-game:
1. Go to your game directory and open `BepInEx/config/com.atomic.customcostics.cfg` using a text editor.
2. Locate the line `"Cosmetic Dev Tools"` and change its value to `true`.
3. Save and close the file.

### Step 16: Launch the Game
Start up the game and navigate to the cosmetics menu.

### Step 17: Locate Your Cosmetic
Find your new item in the menu list. 

### Step 18: Troubleshooting Initial Scale
If you select your cosmetic and cannot see it, it is likely because the 3D model scale is far too large. 
* **Fix:** Use the developer tools overlay to change the scale to `0.02` on the X, Y, and Z axes.

### Step 19: Fine-Tune Positioning
Use the in-game developer tools to adjust the item. You can click directly into the position, rotation, and scale boxes and type values manually to align it perfectly with the character model.

---

## Finalizing and Publishing

### Step 20: Transfer Values Back to Unity
Once you are happy with how the cosmetic looks in-game, write down or copy the final transformation values.
1. Return to your Unity project.
2. Select your **Cosmetic Definition** asset.
3. In the corresponding slot configuration (Head/Body/Face), manually change the **Position**, **Rotation**, and **Scale** fields to match **EXACTLY** what worked in the in-game dev tools.
4. Re-click **Build Cosmetic** to generate the final, perfectly aligned version.
5. Do one final check to make sure it is spot on

### Step 21: Publish Your Mod!
Package your completed cosmetic file along with a `manifest.json`, `icon.png`, and `README.md`, and upload your new mod to [Thunderstore](https://thunderstore.io/) for the community to enjoy!
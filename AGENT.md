# ATELIER — Hackathon Implementation Guide & AI Agent Rules

> **Project:** Atelier · Observation Mode  
> **Platform:** Windows desktop  
> **Engine:** Godot 4 .NET with C#  
> **Input:** Keyboard, mouse, Huion drawing tablet  
> **Hackathon:** 24 hours  
> **Priority:** Player controls and sketchbook controls, **not** multiple worlds or polished menus  
> **VR:** Out of scope for this hackathon

---

# 0. MANDATORY INSTRUCTIONS FOR ALL AI AGENTS

**Read this section before helping with any part of the project. These rules override convenience or speed.**

## Non-negotiable prohibitions

1. **NO ONE-SHOT CODE GENERATION.** Never provide an entire finished feature, full script, complete game, or multi-file implementation in one response. Do not supply copy-paste-ready solutions that bypass the user's reasoning and learning.
2. **DO NOT WRITE CODE INTO THE PROJECT DIRECTORY.** Do not create, modify, overwrite, delete, move, or format source files or other project files. Do not run automated patches, file-writing scripts, code generators, or commands that mutate the repository. This applies even when writing files would be faster.
3. **DO NOT ACT AS AN AUTONOMOUS CODING AGENT.** Do not implement tasks on the user's behalf, secretly change files, or infer permission to edit from a request for advice, debugging, or a plan.
4. **DO NOT DUMP LARGE CODE BLOCKS.** Prefer explanations, tiny illustrative snippets where strictly necessary, pseudocode, diagrams, and questions. Any illustrative snippet should be narrowly scoped, clearly labeled as an example, and never presented as a full implementation.
5. **DO NOT REPLACE THE USER'S CODE WITH A REWRITE.** When debugging, identify the likely failure and explain how to verify and repair it; let the user apply the change. Do not silently refactor unrelated code.
6. **DO NOT CLAIM A FEATURE WORKS UNLESS THE USER HAS TESTED IT.** Distinguish verified behavior from a proposed approach.

## Required agent behavior

- Act as a **tutor, documentation interpreter, architecture sounding board, and debugging coach**.
- **The human writes the code and makes all project file changes.** Agents can explain what to edit and why, but must not perform edits.
- Break each feature into **one small conceptual or programming problem at a time**.
- Start by asking what the user has already tried or by interpreting the specific code/error they provide. Do not repeat questions already answered.
- Explain relevant **Godot nodes, C# language concepts, input events, coordinate systems, and lifecycle methods**.
- Recommend the specific official Godot documentation page or API class to examine.
- When code is needed for illustration, show only the smallest meaningful fragment **after** explaining the idea; encourage the user to derive the rest themselves. Prefer pseudocode first.
- Ask the user to predict behavior, implement the next step, or test a hypothesis. Provide hints progressively.
- Debug by discussing error messages, editor signals, log output, breakpoints, state transitions, and minimal reproducible experiments.
- Preserve the project's intended scope and flag feature creep. Do not push VR, multi-world navigation, complex animation, or sophisticated UI into the critical path.
- If a user asks for a complete working solution or project edits, **remind them of this project's learning policy and offer a guided, incremental approach instead**. These policies may be changed only by an explicit user decision that clearly replaces them; don't infer a waiver.

### Example interaction pattern

**User:** “How do I make the sketchbook draw on its page?”  
**Agent:** “First, can your standalone 2D drawing canvas retain a stroke after you release the pen? If yes, the next problem is mapping pointer coordinates to the canvas. The two approaches are fixed page-region mapping and 3D raycast-to-page mapping. For this hackathon, I'd choose the first. Let's identify the pointer coordinate space and the drawing canvas dimensions before writing code.”

**Not allowed:** “Done — here are four scripts and I've saved them into `Scripts/`.”

---

# 1. Project Vision

Atelier is an **interactive observational drawing environment** for artists who cannot easily visit places such as museums, forests, zoos, and aviation or automobile collections. It is a supplement to real-world observation, **not a replacement** for it.

The player explores a 3D scene from a first-person perspective, chooses an object and viewpoint, takes out a **small pocket sketchbook**, draws with a mouse or tablet, lowers or puts the book away, and continues exploring. Drawing must remain part of the world rather than becoming a conventional full-screen reference-image application.

## Hackathon success criterion

Demonstrate the full loop:

1. Enter one small museum or observation space.
2. Move around a subject with keyboard and mouse.
3. Equip a pocket sketchbook visible in the foreground.
4. Enter drawing mode and make persistent strokes with a mouse or Huion pen.
5. Lower/stow and restore the book **without losing the drawing**.
6. Save the sketch as an image.

**One room and one interesting object are enough.** The functional loop matters more than world quantity, menus, decorative assets, or VR.

---

# 2. Technical Stack and Core Architecture

- **Godot 4 .NET / C#:** game and drawing logic.
- **Blender:** simple sketchbook mesh and environmental assets; export `.glb` where useful.
- **Huion tablet:** test as pointer/stylus input on Windows; treat pressure sensitivity as optional.
- **Git:** use frequent small commits. AI agents may explain Git, but must not mutate repository contents.

## Suggested scene tree (conceptual, adjust as needed)

```text
Main.tscn
├── World (Node3D)
│   ├── Environment
│   ├── Museum
│   └── Exhibits
├── Player (CharacterBody3D)
│   ├── CollisionShape3D
│   └── Camera3D
│       └── SketchbookAnchor (Node3D)
├── DrawingViewport (SubViewport)
│   └── DrawingCanvas (Control)
└── UI (CanvasLayer)
    └── Instructions
```

Possible responsibilities (not a mandate to write all these scripts immediately):

| Component | Single responsibility |
|---|---|
| `PlayerController` | Movement, mouse look, camera behavior |
| `SketchbookController` | Stowed/held/drawing state and book visibility |
| `DrawingCanvas` | Input points, strokes, rendering |
| `GameManager` (only if needed) | Coordination between systems |

Keep systems independently testable. Avoid unnecessary class hierarchies; the user is a beginner in OOP.

## Minimum C# / Godot knowledge

Classes, fields and properties, methods, inheritance, `override`, enums, `List<T>`, `Vector2`, `Vector3`, node references, input events, `_Ready()`, `_Input()` / `_UnhandledInput()`, `_Process()`, and `_PhysicsProcess()`.

---

# 3. Implementation Roadmap by Feature

## A. First-person player movement — **FIRST**

**Goal:** A player can walk around a basic room, approach a subject, and inspect it from multiple angles.

**Nodes:** `CharacterBody3D`, `CollisionShape3D`, `Camera3D`; a `StaticBody3D` / collision for floor and walls.

**Conceptual flow:**

```text
Read WASD input
→ form a 2D direction
→ convert it to movement relative to player orientation
→ set 3D velocity
→ use physics movement (MoveAndSlide)
```

- Horizontal mouse motion turns the player around the Y axis.
- Vertical mouse motion pitches the camera, with a sensible clamp.
- Capture the cursor during exploration.
- Start with flat ground and one cube. Don't begin with elaborate level design.

**Done when:** movement is camera-relative, wall collisions work, and looking up/down feels controlled.

**Docs:** [CharacterBody3D](https://docs.godotengine.org/en/stable/classes/class_characterbody3d.html), [Input](https://docs.godotengine.org/en/stable/tutorials/inputs/index.html).

## B. Camera and observation — **CORE**

Enable moving around an object and choosing a viewpoint. Optional crouch/lower camera makes perspective changes more visible. Avoid first-person hand rigs and body animation for the MVP.

**Done when:** a player can observe one object from front, rear, side, higher/lower viewpoints.

## C. Basic exhibit interaction — **OPTIONAL**

A camera-directed `RayCast3D` can detect a subject; pressing `E` can display its name. This is nice but should never delay drawing integration.

**Docs:** [Ray-casting](https://docs.godotengine.org/en/stable/tutorials/physics/ray-casting.html).

## D. Sketchbook 3D model — **CORE VISUAL**

In Blender, make a small open book with cover and **one flat page surface** that has usable UVs and a distinct material. Export `.glb` and attach the book under a camera-relative `SketchbookAnchor`.

- Keep the book small in the foreground.
- Use a fixed pose in drawing mode.
- Don't implement individual pages, hand IK, book physics, or detailed page-turn animations.

**Done when:** book stays in view when the camera turns, and it can be shown/hidden reliably.

## E. Sketchbook modes — **CORE INTERACTION**

Represent three mutually exclusive states, ideally with an enum:

| State | Book | Input behavior |
|---|---|---|
| **Stowed** | Hidden | WASD + mouse look |
| **Held** | Visible | Can still look around; no drawing strokes |
| **Drawing** | Visible and stable | Cursor/tablet controls drawing; mouse look disabled |

Suggested mappings (change if usability tests indicate otherwise):

| Control | Action |
|---|---|
| `Tab` | Equip / stow sketchbook |
| `F` | Toggle held / drawing |
| `Esc` | Leave drawing mode |
| Pointer / Huion pen | Draw in drawing mode |
| `Ctrl+S` | Save sketch |

Decide transitions explicitly before coding, especially what happens when `Tab` is pressed during drawing mode. A stowed book should retain its page content.

**Done when:** switching between all three states never creates stray strokes, loses data, or rotates the camera during drawing.

**Docs:** [Mouse and input coordinates](https://docs.godotengine.org/en/stable/tutorials/inputs/mouse_and_input_coordinates.html).

## F. Standalone 2D drawing canvas — **HIGHEST TECHNICAL PRIORITY**

**Build this independently before trying to draw directly onto the 3D book.**

**Node:** a `Control` that handles pointer events and custom drawing.

**Relevant concepts/APIs:**

- `InputEventMouseButton`: stroke start / end.
- `InputEventMouseMotion`: pointer movement and (when supported) pen data.
- `Vector2`: 2D points.
- `List<Vector2>`: points per stroke.
- `_Draw()` and `DrawLine()` or `DrawPolyline()`: rendering.
- `QueueRedraw()`: request another render.

**Conceptual flow:**

```text
Press → start new stroke and record first point
Move while pressed → append point; request redraw
Release → finish stroke
Draw pass → render every saved stroke as connected segments
```

Keep strokes separated so lifting and repositioning the pen **does not** draw a line between two strokes. For an initial demo, fixed-width dark strokes are enough.

**Huion test checklist:**

- [ ] Stylus moves pointer inside Godot.
- [ ] Tip contact begins a stroke.
- [ ] Lift ends it cleanly.
- [ ] Subsequent strokes persist.
- [ ] Cursor coordinates match rendered stroke location.

Pressure and tilt are stretch goals, contingent on driver/device compatibility.

**Done when:** a user can draw multiple independent visible strokes using both mouse and pen.

**Docs:** [Custom drawing in 2D](https://docs.godotengine.org/en/stable/tutorials/2d/custom_drawing_in_2d.html), [InputEventMouseMotion](https://docs.godotengine.org/en/stable/classes/class_inputeventmousemotion.html).

## G. Display the drawing on the 3D page — **CORE INTEGRATION**

Treat these as **two distinct problems**:

1. **Display:** a `SubViewport` renders a 2D drawing canvas; its `ViewportTexture` can be displayed by a 3D page material.
2. **Interaction:** mouse/tablet coordinates must correctly correspond to positions on the visible page.

**Preferred hackathon approach:** Keep the page and camera stable in drawing mode and map the input to a known, rectangular page area. A camera-attached, book-shaped 2D drawing surface that visually matches the book is a valid fallback.

**Advanced alternative:** cast a ray through the screen cursor, intersect a page plane/collision shape, convert the hit to local page coordinates, and map those coordinates into the drawing texture. Defer unless the simpler pipeline works early.

**Done when:** drawn strokes appear on the visible page, at the expected stylus positions, and survive mode changes.

**Docs:** [Using a viewport as a texture](https://docs.godotengine.org/en/stable/tutorials/shaders/using_viewport_as_texture.html), [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html).

## H. Pencil / pen / eraser — **INCREMENTAL**

Recommended implementation order:

1. Single fixed-width dark pencil stroke.
2. Adjustable width.
3. Optional dark pen variant.
4. Eraser (simplest reasonable behavior: delete whole touched stroke, or visibly paint over with paper color with known limitations).
5. Pressure sensitivity **only if time remains**.

Do not attempt physics-based ink, textured graphite, true watercolor, layer compositing, or complex brush engines in 24 hours.

## I. Save sketch — **CORE**

When the user invokes save:

```text
Receive save command
→ obtain final drawing image
→ choose safe filename / user location
→ export PNG
→ check for errors
→ display success/failure
```

A drawing viewport texture can be converted to an `Image` and saved with Godot's PNG support. Ensure the viewport has rendered before capture. Test the saved image by opening it outside the game. Later, saving stroke data separately could support editing, but not necessary for MVP.

**Done when:** a PNG exists and visibly contains the sketch after the application closes.

**Docs:** [Image](https://docs.godotengine.org/en/stable/classes/class_image.html).

## J. Environment / museum — **SMALL SCOPE**

- One walkable space, one compelling subject, optionally two or three extras.
- Floor/wall collision and straightforward lighting.
- Import Blender or properly licensed assets.
- Favor good object placement and multiple viewing angles over complex models.

**Done when:** the player can approach, circle, and observe the subject, then draw it.

## K. Home screen / world select / library — **DEFER**

| Screen | Hackathon decision |
|---|---|
| Home | Simple Start and Quit if time permits |
| World select | Skip with only one scene |
| Gameplay | Central experience; essential |
| Library | Skip initially; exported PNGs are enough |

## L. Sound and ambience — **POLISH**

Optional `AudioStreamPlayer` for a subtle museum ambience or menu audio; use licensed files. No advanced audio architecture.

## M. VR — **OUT OF SCOPE**

Do not let OpenXR, Quest 2, controllers, XR locomotion, or VR drawing enter this hackathon's critical path. Revisit only after desktop exploration and sketching work end to end.

---

# 4. Time-Boxed 24-Hour Hackathon Plan

| Time | Milestone | Exit criterion |
|---|---|---|
| **0–3h** | Project / first-person room | Walk, look, collide |
| **3–8h** | Independent 2D drawing | Mouse and Huion pen leave persistent strokes |
| **8–13h** | Sketchbook model and states | Book visible/stowable; input modes reliable |
| **13–18h** | Drawing-to-page integration + export | Book displays strokes; PNG export works |
| **18–21h** | Exhibit and ambience | At least one appealing draw-able subject |
| **21–24h** | Testing, packaging, presentation | Stable Windows build and rehearsed demo |

**Hard fallback:** If drawing on the real 3D page is not reliable by about hour 13, use a fixed camera-attached book-shaped **2D canvas** instead. Do not sacrifice an end-to-end working demo for technically pure 3D surface mapping.

Reserve time for testing, including a Windows build. Don't spend the final hours implementing world selection, a library, or extra exhibit rooms.

---

# 5. Team Collaboration

If working in a team, split initially independent work:

| Workstream | Output |
|---|---|
| Player controls | 3D navigation, camera, input states |
| Drawing | Canvas, strokes, tablet testing, PNG export |
| Assets / world | Book mesh, one room, exhibits, lighting |
| Integration / testing | Coordinate systems, shared scene, reproducible QA |

With two people: one handles player/world, one handles drawing/book; integrate together. Human team members make all commits and file edits. Coordinate before touching shared scene files.

---

# 6. Agent-Assisted Learning Workflow

## For unknown concepts

> “Explain this Godot 4 C# documentation section in beginner-friendly terms. Define the unfamiliar words and connect them to my player/sketchbook project. Do not implement it for me.”

## For planning a feature

> “Break **[feature]** into the smallest programming decisions. Identify the Godot nodes and APIs to investigate. Ask me to reason through the next step. No complete code; do not change any files.”

## For debugging

> “Here is my code and error. Help me isolate the first failed assumption. Suggest logs, breakpoints, or a tiny experiment. Do not rewrite the whole script or edit the repository.”

## For reviewing code

> “Review this code for conceptual mistakes, lifecycle problems, and coordinate-space errors. Explain each issue and let me apply the edits. Do not output a fully replaced implementation.”

## Recommended guidance loop

1. Identify **one** small feature or bug.
2. Read the relevant docs and articulate the behavior wanted.
3. Ask the agent for conceptual explanation, not a completed script.
4. **Human writes** the change.
5. Run and test it.
6. Share precise errors or observed behavior for feedback.
7. Repeat.

If an agent suggests writing files or generating the whole feature, **stop it and redirect it to this section**.

---

# 7. Documentation Bookmarks

- [Godot C# basics](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html)
- [CharacterBody3D](https://docs.godotengine.org/en/stable/classes/class_characterbody3d.html)
- [Input event reference](https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html)
- [Mouse and input coordinates](https://docs.godotengine.org/en/stable/tutorials/inputs/mouse_and_input_coordinates.html)
- [Custom drawing in 2D](https://docs.godotengine.org/en/stable/tutorials/2d/custom_drawing_in_2d.html)
- [InputEventMouseMotion](https://docs.godotengine.org/en/stable/classes/class_inputeventmousemotion.html)
- [Using a viewport as a texture](https://docs.godotengine.org/en/stable/tutorials/shaders/using_viewport_as_texture.html)
- [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html)
- [Image](https://docs.godotengine.org/en/stable/classes/class_image.html)
- [Ray-casting](https://docs.godotengine.org/en/stable/tutorials/physics/ray-casting.html)

---

# 8. Definition of Done and Scope Guardrails

## Essential

- [ ] First-person navigation in one 3D room.
- [ ] A subject can be examined from different viewpoints.
- [ ] Pocket sketchbook can be equipped and put away.
- [ ] Drawing mode prevents unwanted camera movement.
- [ ] Mouse and Huion pen can draw persistent strokes.
- [ ] Sketch survives equipping/stowing transitions.
- [ ] Sketch can be saved as PNG.
- [ ] Windows demo launches and can be completed without developer assistance.

## Stretch goals only

- [ ] Brush width, alternate pens, eraser.
- [ ] Simple exhibit labels.
- [ ] Crouch / observation viewpoints.
- [ ] Gentle audio ambience.
- [ ] Home screen.
- [ ] Better animations and visual polish.

## Do not pursue during this event

- [ ] VR support.
- [ ] Multiple elaborate worlds, world select.
- [ ] In-game drawing library UI.
- [ ] Multiplayer.
- [ ] Simulated realistic ink or watercolor.
- [ ] Complex physical hand/pen interaction.
- [ ] Advanced movement / character animations.

> **Guiding principle:** Preserve the interactive loop **explore → observe → equip sketchbook → draw → stow → continue exploring → save**. The learning process is part of the project's success. **Agents teach; humans code. Agents do not write into the project directory.**

# BigGameSecurity Product Requirements Document

*Version:* 1.0  \
*Date:* October 19, 2025  \
*Target Platform:* Windows & macOS  \
*Game Engine:* C#  \
*Genre:* Management Simulation, Strategy

## 1. Executive Summary
BigGameSecurity is a 2D isometric stadium security management game where players act as the Security Director overseeing gate operations during a major sporting event. The core gameplay focuses on strategic planning and hiring decisions rather than real-time action. Players must hire qualified security personnel, assign them to 12 stadium gates, and manage the 6-hour event window (18:00-24:00) to maintain operational efficiency and reputation. Success depends on preparation: hiring the right staff, understanding their strengths and weaknesses, assigning them strategically, and timing gate operations correctly. The game embraces a deliberate pace where waiting is a core mechanic that produces tension through anticipation rather than action.

## 2. Game Vision & Core Pillars
### 2.1 Vision Statement
A management simulation that rewards planning, patience, and personnel judgment over reflexes and micromanagement.

### 2.2 Core Pillars
- **Preparation Over Execution** – 80% of success is determined before the event starts.
- **Consequence-Based Gameplay** – Hiring and assignment decisions play out automatically during the event.
- **Slow Burn Tension** – Waiting and watching is the primary activity.
- **No Win, Only Survival** – Continue as long as reputation stays above zero.

### Accessibility Considerations
- Simple mouse plus keyboard controls.
- No time pressure during the planning phase.
- Clear visual feedback for automated events.
- No difficulty modes (single balanced experience).
- No audio (for now).

## 3. Gameplay Overview
### 3.1 Game Loop Structure
1. **Preparation (Pre-Event)** – Review event documentation, browse applicant CVs, hire at least 12 security staff, assign them to 12 gates, review stats and assignments, review gate schedules, and confirm readiness.
2. **Event Execution (18:00-24:00)** – Monitor gate and staff performance, open and close gates at correct times, observe automated security processes, and respond to major incidents through reassignments if needed.
3. **Post-Event** – Review the performance report, analyze reputation changes, fire or retain staff, and prepare for the next event.

### 3.2 Core Gameplay Mechanics
#### Hiring System
Players browse a randomized stack of applicant CVs displaying:
- First Name, Last Name
- Age, Gender
- ID Number (flavor text)
- Physical Strength (1-10)
- Communication (1-10)
- Observation (1-10)
- Reliability (%)
- Focus Sustainability (%)
- Quit Risk (%)

Hiring actions include **Accept** (add to roster) and **Reject** (discard and view next CV).

#### Staff Assignment
- Assign staff members to 12 gate positions (minimum one per gate).
- Gates can have multiple staff to reduce individual workload.
- Unassigned staff serve as reserves available for reassignment during the event.

#### Gate Management
- Manual controls: Open/close individual gates, open/close all gates, lock gates to prevent accidental changes.
- Sample timing requirements: External gates open at 18:00, security gates at 18:45, keep open through 22:00, close by 23:00, final sweep completes by 24:00.

#### Automated Security Processing
Automated systems based on staff stats handle ticket checking, ID verification, bag checks, body checks, and incident response. Multiple staff at a gate combine their effectiveness.

#### Guest Flow System
Guest journey states include: At Home → At Stadium Gates → Walking to Security → At Security → Watching Game, with alternate states such as At Jail, At Hospital, Hiding After Game, Fighting with Security, and Fighting with Other Guests.

Guest attributes include demographics, Strength, Aggression, and flags for Fake ID, Fake Ticket, and Major Threat Entity (MTE) status. Guest volume starts at 200-500 individuals for playability.

#### Time System
- Real-time to game-time ratio: 1 real second equals 6 in-game minutes.
- Event timeline milestones: External gates open at 18:00, security gates at 18:45, game starts 20:30, halftime at 21:15, game ends 22:00, gates close 23:00, final sweep complete 24:00.
- No pause or fast-forward options; the full event lasts roughly 60 real-time minutes.

### 3.3 Risk Events (Automated)
Automated events include fake tickets, fake IDs, no ID, MTEs, aggressive fights, medical emergencies, riots, gate mishandling, political reputation hits, and inefficient organization. Outcomes depend on staff effectiveness and player gate management decisions.

## 4. Reputation System
- **Starting Reputation:** 100 points.
- **Reputation Gains:** Achieved by high throughput, detecting and stopping MTEs, preventing riots, fast medical responses, and smooth gate operations.
- **Reputation Losses:** Result from long queues, missed fake tickets/IDs, MTEs entering, escalated riots, ignored medical emergencies, mistimed gate operations, staff no-shows or quits, and security breaches after 24:00.
- **Lose Condition:** Reputation reaching 0 triggers a game over where the player is fired and restarts with a fresh CV stack and reset reputation.
- **No Win Condition:** The game continues indefinitely as long as reputation remains above zero. Success is measured by consecutive events survived and highest reputation achieved.

## 5. User Interface Requirements
### Visual Style
- Isometric 2D reminiscent of late-1990s management sims.
- Professional corporate color palette with alert highlights.
- Clear visual differentiation between staff, guests, and gate states.

### Camera & View
- Fixed isometric view with no rotation or zoom.
- Layout positions external gates at the top of the screen and interior seating at the bottom.
- All 12 gates remain visible simultaneously.

### HUD Elements
- **Top Bar:** Displays current in-game time, reputation score, and a phase indicator.
- **Per-Gate Information:** Shows gate number, queue length, assigned staff names, staff focus percentage (color-coded), and gate status (open/closed indicator).
- **Event Log:** Side panel with color-coded feed for routine, warning, and critical events.
- **Bottom Control Bar:** Controls for opening/closing gates, individual gate toggles, and a staff roster button.

### Preparation Phase UI
- **CV Browser:** Displays current CV with Accept/Reject actions, hired staff counter, compare CVs option, and finish hiring button.
- **Staff Assignment:** Drag-and-drop interface for assigning staff to gates with confirm and back navigation.
- **Event Briefing:** Presents event details, attendance expectations, risks, gate schedules, and the start event button.

### Post-Event UI
- Performance report summarizing final reputation, change from starting value, total guests processed, incidents prevented/missed, and staff performance ratings, with options to continue to next event or manage staffing.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovementScript : MonoBehaviour
{
    ///                                  <--- Created By Kaleb Clint --->
    ///                                        
    ///
    ///                                             <--SUMMARY->
    /// 
    /// 
    ///   This is my own First Person Character Controller Script. Some of it I have
    ///   taken from tutorials and adapted to fit the script. Just to make it much easier
    ///   whenever I, or anyone else starts a game they can get this part over with quickly. 
    ///   I will clearly state where a feature is and what you need to do to set it up in unity itself. 
    ///   I will also tell you what to delete if you don't want a specific features.
    ///   
    ///   This uses the character controller component, so features such as climbing slopes and
    ///   stairs are built in.
    ///   
    ///                                             <--FEATURES-->
    /// 
    /// 1. Basic Movment
    ///    - WASD and Space.
    ///    - Can Climb Slopes / Stairs.
    ///    - Editable Values 
    ///        - Speed, Jump Strength, Gravity Strength
    ///        - Slope Hieght, Step Height + more
    ///    
    /// 2. Gravity / Jumping
    ///    - Can Change Strength (Gravity + Jump)
    ///    
    /// 3. Double Jump 
    ///    - Editable number of extra jumps.
    ///    - Editable strength of extra jumps.
    ///    - Unlockable.
    ///    
    /// 4. Crouching
    ///    - Crouching is both Toggle and hold. (As in tap to toggle, or if you want hold.)
    ///    - Can edit height and crouching speed.
    ///    - Unlockable.
    /// 
    /// 5. Sprinting
    ///    - (optional) Acceleration Toggle 
    ///    - (optional) FOV effect
    ///    - (optional) Stanima (Note, there is no GUI for it, you will have to do that. (: )
    ///        - regenerates once you run out. (Can edit speed, such as making it slower if 
    ///          you run out, verus normal regenrations)
    ///        - (optional) Player is 'exhausted' once stanima runs out and until its completely regenerated.
    ///        - (optional) Players speed is corelated to the stanima. (lower stanima, slightly lower speed.)
    ///    - Editable Values
    ///        - Sprinting Speed
    ///        - Acceleration Time
    ///        - Stanima --- Regen Speed, Amount, Stanima Loss Speed, How much it affects speed.
    ///    - Unlockable
    ///    
    /// 
    ///                                             <--HOW TO USE-->
    /// 
    /// 
    ///     1. Create an empty and name it Player or whatever you like. Add a 'Character Controller' Component.
    ///        You can keep most of this settings the same or tweak them later to fit your needs. 
    ///     
    ///     2. Give the player a mesh body (Whatever you like, a cube, a cylinder, or your own model) 
    ///    
    ///     3. Then give the player a camera. If you already have a main camera in your scene, you can just put it
    ///        as a child of the player.
    ///        
    ///     4. Add this script to Player, then add the CameraLook script to the player. This is the basic setup,
    ///        got through the features below to implement them.
    ///
    ///     5. Create a simple empty object and place it at the bottom of the player. Name it groundcheck and make 
    ///        sure it is a child of the player object. Then drag the groundcheck into the players 'Ground Check'
    ///        in the script component.
    ///        
    ///     6. Any new features you'll have to play around and add manualy. However if there are features you do not
    ///        wish to be here then go through and delete them where it says to. I've marked say everywhere there is a
    ///        double jump script. If you miss something it'll just give you an error pointing to it anyway.
    ///         
    ///  Tutorials:
    ///  FIRST PERSON MOVEMENT in Unity - FPS Controller || By Brackeys
    ///  First Person Controller - Crouching (EP04) [Unity Tutorial] || By Comp-3 Interactive
    ///
    ///

    #region Variables

    //Variables are relatively self explanetory in their uses. 
    //No need to change anything here (unless you are removing features.)
    //You can tweak stuff in the editor.

    #region Controls

    [Header("Controls")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;

    #endregion

    //Keep, do not delete. Script is prob useless without it.
    #region Gravity, Movement, and Jump.

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float gravity = -15;
    [SerializeField] private float jumpStrength = 3f;

    [SerializeField] private Camera cam;
    [SerializeField] private CharacterController controller;

    //Speed is set in the update function based on player action. (walk,run,crouch)

    [Tooltip("Normal speed is before its affected by outside things, such as stanima. This is based on whether sprinting/walking/crouching")]
    [SerializeField] private float normalSpeed;
    [Tooltip("End Speed is after outside variables are taken into account, such as say stanima or powerups.")]
    [SerializeField] private float endSpeed;



    private Vector3 velocity;
    private bool isGrounded;

    #endregion

    //Delete if you don't want crouching.
    #region Crouching

    [Header("Crouching")]

    [SerializeField] private bool canCrouch = true;
    [SerializeField] private float crouchSpeed = 3;


    [SerializeField] private float crouchHeight = 0.5f;

    //Change to the hieght of your player
    [SerializeField] private float standingHeight = 2.5f;

    [SerializeField] private float timeToCrouch = 0.25f;

    // Center of Player Controller. Changing to keep camera centered with collider.
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);

    // This StandUp bool is just to fix a slight bug. (when player holds crouch and goes
    // under something, they dont stand up when they leave. Without this bool.)
    private bool standUp;

    private bool isCrouching;
    private bool duringCrouchAnimation;

    #endregion

    //Delete if you dont want double jump.
    #region Double Jump

    [Header("Double Jump")]

    [SerializeField] private bool canDoubleJump = true;

    private bool doubleJumped;

    //Max jumps is how many times the player can jump before hitting the ground.
    //(1 max jump is just a normal double jump)
    [SerializeField] private int maxJumps = 1;
    private int jumps;

    //Same as normal jump. You can change if you want.
    [SerializeField] private float doubleJumpStrength = 3;

    #endregion

    //Delete if you dont want sprinting.
    #region Sprinting

    [Header("Sprinting")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool acceleration;
    [SerializeField] private bool stamina;

    [SerializeField] private float sprintSpeed = 15;

    [SerializeField] private float accelerationTime = 1f;

    private bool isSprinting;

    [Header("Field of View")]
    [Tooltip("If the player is running, changes the cameras field of view dependant on the speed.")]
    [SerializeField] private bool FOVeffect = true;
    [SerializeField] private float FOVstrength = 5;
    [SerializeField] private float normalFOV = 60;
    [SerializeField] private float FOVspeed = 0.2f;


    #region Stanima

    [Header("Stanima")]
    [SerializeField] private bool canRun;
    [SerializeField] private float staminaAmount = 100;
    [SerializeField] private float maxStanima = 100;
    [SerializeField] private float stanimaUsage = 12;
    [SerializeField] private float stanimaRegenSpeed = 11;

    [Header("Exhaustion")]

    [Tooltip("If ticked, once the player uses all stanima, they can't run until stanima fully regenerates.")]
    [SerializeField] private bool Exhaustion;

    [SerializeField] private bool isTired;
    [Tooltip("Regeneration Speed for when player is exhausted")]
    [SerializeField] private float tiredRegenSpeed = 9;

    [Tooltip("How slower the player is when exhausted (2 is halved, 1 is no effect.)")]
    [SerializeField] private float speedLoss = 1.5f;

    [Header("Speed and Stanima Corelation")]
    [Tooltip("If ticked, the players speed will be impacted by total amount of stanima")]
    [SerializeField] private bool speedStanimaCorelation;
    [Tooltip("Affects how much it is corelated. (Greater the number, less the corelation)")]
    [SerializeField] private float corelationAmount = 15;


    #endregion

    #endregion

    #endregion

    //\\//\\//\\

    #region Update / Start Function

    private void Start()
    {
        if (Exhaustion) { isTired = false; }
        cam.fieldOfView = normalFOV;
    }

    // Update is called once per frame
    void Update()
    {

        endSpeed = normalSpeed;
        if (isTired) { endSpeed = endSpeed / speedLoss; }

        if (speedStanimaCorelation) { endSpeed = endSpeed + ((staminaAmount - 100) / corelationAmount); }


        // Gravity, Movement, and Jump. (And DoubleJump)

        #region Ground Check / double jump


        //uses Groundcheck object to see if player is on the ground.
        isGrounded = controller.isGrounded;
        //Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //If player is on the ground, resets the fall velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            //delete if you dont want double jump
            doubleJumped = false;
            jumps = 0;
        }


        #endregion

        #region WASD movement

        //Nothing needed to do here

        //Getting the inputs for WASD
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Moving the player
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * endSpeed * Time.deltaTime);

        #endregion

        #region Excecute Gravity / double Jump


        // normal jump
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            //Jumping using physics equation ( v = (square root of) h * -2 * g )
            velocity.y = Mathf.Sqrt(jumpStrength * -2 * gravity);
        }


        // Delete This if you don't want double jump
        #region DOUBLE JUMP

        /// Set canDoubleJump to false if you want the player to temporaily not be able to
        /// doubleJump. Ex. if its something they unlock later. Set it to true once they 
        /// unlock it.

        if (Input.GetKeyDown(jumpKey) && !isGrounded && !doubleJumped && canDoubleJump)
        {
            //Jumping using physics equation ( v = (square root of) h * -2 * g )
            velocity.y = Mathf.Sqrt(doubleJumpStrength * -2 * gravity);

            // Add a jump
            jumps++;

            //If they have jumped as many times as max, can no longer jump till hits ground.
            if (jumps >= maxJumps)
            {
                doubleJumped = true;
            }

        }

        #endregion

        //Making the player fall by increasing fall velocity then moving player with velocity.
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        #endregion

        //Delete if you dont want crouching
        #region Crouching

        if (isCrouching) { normalSpeed = crouchSpeed; }

        //Fix crouching Bug
        if (!Input.GetKeyDown(crouchKey) && standUp && !Physics.Raycast(cam.transform.position, Vector3.up, 1f))
        {
            standUp = false;
            StartCoroutine(CrouchStand());
        }

        if (canCrouch)
        {
            handleCrouch();
        }

        #endregion

        //Delete if you dont want sprinting
        #region Sprinting

        if (canSprint && !isCrouching) { handleSprinting(); }

        #endregion

    }

    #endregion

    //Delete if you dont want crouching
    #region Crouching

    private void handleCrouch()
    {

        if (Input.GetKeyDown(crouchKey) || Input.GetKeyUp(crouchKey) && !duringCrouchAnimation && isGrounded)
        {
            StartCoroutine(CrouchStand());
        }

    }

    private IEnumerator CrouchStand()
    {

        if (isCrouching && Physics.Raycast(cam.transform.position, Vector3.up, 1f))
        {
            if (Input.GetKeyUp(crouchKey))
            {
                //Make them stand up when they are not under something
                standUp = true;
            }

            yield break;

        }

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = controller.height;

        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        if (isCrouching) { normalSpeed = crouchSpeed; }
        else if (!isSprinting) { normalSpeed = walkSpeed; }

        duringCrouchAnimation = false;
    }


    #endregion

    //Delete if you dont want sprinting
    #region Sprinting

    private void handleSprinting()
    {
        if (Input.GetKeyDown(sprintKey) && !isCrouching && !isTired && canRun)
        {
            isSprinting = true;
            if (acceleration) { StartCoroutine(SprintAcceleration()); }

            else { normalSpeed = sprintSpeed; }
            
        }
        if (Input.GetKeyUp(sprintKey) || !canRun && !isCrouching || !canRun)
        {
            isSprinting = false;

            if (acceleration) { StartCoroutine(SprintAcceleration()); }

            else { normalSpeed = sprintSpeed; }
        }

        if (FOVeffect) { fovHandler(); }

        if (stamina){ StartCoroutine(staminaHandler()); }

    }

    #region Stamina

    IEnumerator staminaHandler()
    {
        if (isSprinting)
        {
            if (staminaAmount > 1) {staminaAmount -= stanimaUsage * Time.deltaTime; }    
            
            else { canRun = false; if (Exhaustion) { isTired = true; } }
        }

        if(!isSprinting) 
        {

            yield return new WaitForSeconds(1);

            if(staminaAmount > 1) { canRun = true; }

            if(staminaAmount < 100)
            {
                if (!isTired) { staminaAmount += stanimaRegenSpeed * Time.deltaTime; }

                else { staminaAmount += tiredRegenSpeed * Time.deltaTime; }
            }
            else { isTired = false; }
        }

        #region FOV

        #endregion

    }

    #endregion

    #region Acceleration

    private IEnumerator SprintAcceleration()
    {
        float timeElapsed = 0;
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        float currentSpeed = normalSpeed;

        while (timeElapsed < accelerationTime && !isCrouching)
        {
                normalSpeed = Mathf.Lerp(currentSpeed, targetSpeed, timeElapsed / accelerationTime);
                timeElapsed += Time.deltaTime;
                yield return null;
        }

        if (isCrouching) { normalSpeed = crouchSpeed; }
        normalSpeed = targetSpeed;

    }

    #endregion

    #region FOV
    private void fovHandler()
    {

        if (isSprinting) { if (cam.fieldOfView <= (endSpeed * FOVstrength)) { cam.fieldOfView += FOVspeed; } }

        else { if (cam.fieldOfView >= normalFOV) { cam.fieldOfView -= FOVspeed; } }

    }

    #endregion

    #endregion

}
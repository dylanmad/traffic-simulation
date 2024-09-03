# Unity 3D Traffic Simulation 

# About
Using the design principles behind Finite State Machines (FSM), I implemented a 3D traffic simulation in Unity using C#. The simulation includes vehicles and pedestrians, as well as various traffic management devices like traffic signals, crosswalks, stop signs, etc. The scene encapsulates four square blocks of a city, and vehicles are able to make their own decisions on whether or not to turn at the designated intersections. Pedestrians are able to walk along the sidewalks that surround each square block and are able to reactively use crosswalks when the proper traffic signal is displayed. Finally, both vehicles and pedestrians use ray casting for collision detection and avoidance.

# Features
## Traffic Signals & Crosswalks
The simulation includes an intersection with functional traffic signals that display green, yellow, and red lights. The traffic signals are associated with a state depending on which axis they are aligned with, and both vehicles and pedestrians are responsive to this state. The simulation also features four crosswalks surrounding the main intersection where pedestrians are able to cross based on which traffic signal is currently active.
<p align="center">
<img src="https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExc3BieDNlM2h4YmFlNHJiZmRqYXdnY3M5M3AwYXNtYzQ4bDFqNGtkZCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/uT7YGhi4MahGi59wr3/giphy.gif" alt="Main Intersection" width="800"/>
</p>

## Collision Detection
Both vehicles and pedestrians use ray casting for collision detection. When an object is within the ray being casted, the vehicle or pedestrian will stop until the object leaves the target area.
<p align="center">
<img src="https://i.imgur.com/7IXfES6.png" alt="Ray Casting Example" width="800"/>
</p>

## Stop Signs
The simulation also features four stop signs where vehicles will stop for three seconds before turning right.
<p align="center">
<img src="https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExNWx2NWthbHpzOWh4NjYzOWptdGw5dGI5YXJxa2hscGk0N2w5bnRvayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/Sz8jH3DPjPqjs3X7c1/giphy.gif" alt="Stop Sign" width="800"/>
</p>

In addition to these features, vehicles and pedestrians are able to turn left and right. Vehicles have the ability to randomly decide whether to turn or continue driving forward at designated sections located throughout the simulation.

# Implementation

I wrote various scripts that determine the state of each object and make decisions based on the current state. Below is part of the script attached to each vehicle. In this method, I check the tag of the street section that the vehicle has last interacted with, and make a decision based on the tag. I also incorporate randomness for left/forward turns.
```csharp
 // sets 'decision' variable based on street tag which the vehicle interacts with
 private void OnCollisionEnter(Collision collision)
 {

     // default forward movement
     if (collision.gameObject.tag == "street")
     {
         decision = "forward";
     }
     // random left/forward turn
     if (collision.gameObject.tag == "leftForward")
     {
         random = UnityEngine.Random.Range(-1f, 1f);

         if (random > 0)
         {
             decision = "left";
             turn = "left";
         }
         else
         {
             decision = "forward";
             turn = "default";
         }

     }
    ...
```

In the Update method, I use the 'decision' variable representing the vehicle state to determine what action should be completed. Depending on the state, an associated method is called.
```csharp
// decides which method to run depending on 'decision' value and if car is colliding with another car
private void Update()
{

    DetectVehicle();

    if (decision == "forward" && !collide)
    {
        DoForward();
    }
    if (decision == "left" && !collide)
    {
        TurnLeft();
    }
    if (decision == "right" && !collide)
    {
        TurnRight();
    }
    // 'active' prevents the coroutine from running every frame
    if (decision == "wait" && !collide && !active)
    {
        StartCoroutine(RedLight());
    }

}
```

Vehicle collision detection is implemented using ray casting, which sets the vehicle velocity to zero when the vehicle is within range of another object. Pedestrian collision detection is implemented in a similar way.
```csharp
// uses ray casting to detect vehicles and then stops if detected
private void DetectVehicle()
{

    Vector3 currentPosition = transform.position;
    currentPosition.y += 1f;
    Vector3 ray1 = Quaternion.Euler(0, -20, 0) * transform.forward;
    Debug.DrawRay(currentPosition, ray1 * 5f, Color.red);
    Vector3 ray2 = Quaternion.Euler(0, 0, 0) * transform.forward;
    Debug.DrawRay(currentPosition, ray2 * 7f, Color.red);
    Vector3 ray3 = Quaternion.Euler(0, 20, 0) * transform.forward;
    Debug.DrawRay(currentPosition, ray3 * 5f, Color.red);


    if (Physics.Raycast(currentPosition, ray1, out hit, 5f) || Physics.Raycast(currentPosition, ray2, out hit, 7f) || Physics.Raycast(currentPosition, ray3, out hit, 5f))
    {
        if (hit.collider.CompareTag("vehicle"))
        {
            collide = true;
            vehicle.velocity = Vector3.zero;
        }

    }
    else
    {
        collide = false;
    }

}
```

When a vehicle interacts with an intersection, it waits at a red light for one second before reevaluating the current signal state. The signal state is set inside of another script which is imported into the vehicle script.
```csharp
// stops at a red light for 1 second then reevaluates traffic signal state
IEnumerator RedLight()
{
    active = true;
    vehicle.velocity = Vector3.zero;
    yield return new WaitForSeconds(1);

    // evaluates if the light is green, and if so the vehicle state is set to forward
    if (street == 'x')
    {
        if (xSignal.signalState == "green")
        {
            decision = "forward";
        }
    }
    if (street == 'z')
    {
        if (zSignal.signalState == "green")
        {
            decision = "forward";
        }
    }

    active = false;
}
```

There are many more methods and features that I recommend you check out if you want to learn more about how I implemented this simulation, but these were the main ones I wanted to highlight.

# Backlog
Below is a backlog of features, in no particular order, that I intend to implement and deploy in the near future:
## Refined Vehicle/Pedestrian Movement: 
The ability for vehicles and pedestrians to accelerate, deccelerate, and turn smoothly.
## Bus Stop: 
Addition of a bus which stops at designated bus stops throughout the simulation. Bus has the ability to pick up and drop off pedestrians.
## More Immersive Features: 
The addition of sound effects, as well as more complexity will make the simulation more immersive and feel more authentic.

# Acknowledgment
When implementing this simulation, I used two free asset packs from the Unity Asset Store, including [City Package](https://assetstore.unity.com/packages/3d/environments/urban/city-package-107224) and [Simple Cars Pack](https://assetstore.unity.com/packages/3d/vehicles/land/simple-cars-pack-97669) to aid in modelling.

# Conclusion
In closing, I want to thank you for checking out my project, and would like to invite you to watch the demo video located at the beginning of this file for more insight into what my simulation looks like in-action.

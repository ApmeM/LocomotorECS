LocomotorECS
==========
LocomotorECS is a set of helper classes to add ECS to your game.


Entity Systems
============
Entity Systems are an easy way to encapsulate game logic that spans across different entities and components.

Systems are executed in the order you add them to the scene. You might want to add your physics system after your input logic. 
You might want to do all bullet collision checks after the physics system has calculated the new positions. Adding the systems in the proper order allows you to do this.


## Basic systems
The following basic systems provided as an abstract base classes that can be inherited to implement your systems

- EntitySystem - the very basic system that executes each time you call for an action. 
- MatcherEntitySystem - system with filter by entity component that allows you to select only entities that fits some components content criterias
- EntityProcessingSystem - system that iterates throug all matched entities to allow per-entity manipulations


## Matchers
Matchers are used to match entities based on a pattern of components to define what components a system is interested in.

### Match all
Match all the entities that have both Component1 AND Component2.

```cs
new Matcher().All( typeof( Component1 ), typeof( Component2 ) );
```


### Match one
Match one the entities that have at least Component1 or a Component2.

```cs
new Matcher().One( typeof( Component1 ), typeof( Component2 ) );
```


### Match exclude
Match exclude the entities that do not have the Component1.

```cs
new Matcher().Exclude( typeof( Component1 ) );
```


Entity
==========
Entities are a container classes for components to be managed by Entity systems. You can create an Entity instance and add any required Components to it.

Note that component list changes and its availability (enabled / disabled) are take effect only after CommitChanges are called.


Component
==========
Components are added properties for entity. 

Example
==========

```cs
    // First declare a component.
    public class AIComponent: Component
    {
       // Declare some properties here you might need
       public IAITurn AIBot { get; set; }
    }

    // Then declare a entity system that handles the component above
    public class AIUpdateSystem : EntityProcessingSystem
    {
        // In constructor define a filter for components this system handles
        public AIUpdateSystem() : base(new Matcher().All(typeof(AIComponent)))
        {
        }

        protected override void DoAction(Entity entity, TimeSpan gameTime)
        {
            base.DoAction(entity, gameTime);

            // Each step get a handled component
            var ai = entity.GetComponent<AIComponent>();
 
            // And do something with it
            ai.AIBot.Tick();
        }
    }

    // Then in your scene / system base loop
    public class Program
    {
        public static void Main()
        {
            // Initialize entity and system lists
            var entityList = new EntityList();

            // System list is subscribed to entity list.
            // Multiple system lists can be subscribed to the same entity list.
            // This can be useful if you want to split your system into 2 independent loops (for example Update and Draw) or make them updated in a certain order.
            var updateSystems = new EntitySystemList(entityList);
            var drawingSystems = new EntitySystemList(entityList);

            // Add different systems to those lists
            updateSystems.Add(new AIUpdateSystem() );
            drawingSystems.Add(new AIDrawingSystem() );

            // create new entity
            var entity = new Entity("Example AI");

            // And add a component to it
            entity.AddComponent<AIComponent>();

            entityList.Add(entity);

            // Before loop call systems to initialize.
            updateSystems.NotifyBegin();
            drawingSystems.NotifyBegin();
            while (...)
            {
                // In main game loop call update systems and then drawing systems.
                updateSystems.NotifyDoAction(TimeSpan.Zero);
                drawingSystems.NotifyDoAction(TimeSpan.Zero);

                // At the end of the loop commit all entity and components changes.
                entityList.CommitChanges();
            }
            // After the loop call system finalization.
            updateSystems.NotifyEnd();
            drawingSystems.NotifyEnd();
        }
    }

```

##Credits

- [**Nez**](https://github.com/prime31/Nez) - ![GitHub stars](https://img.shields.io/github/stars/prime31/Nez.svg) - 2D game engine.
- [JDK](http://fuseyism.com/classpath/doc/java/util/BitSet-source.html) - BitSet source ported to C#

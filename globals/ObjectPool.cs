using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Game.Autoloads;

public partial class ObjectPool : Node {
    public static ObjectPool Instance { get; private set; }

    private Dictionary<PackedScene, Stack<Node>> pool = new();

    public override void _Ready() {
        Instance = this;
    }

    public Node RequestInstantiate(PackedScene scene) {
        if (pool.TryGetValue(scene, out var existingInstance)) {
            Node instance = existingInstance.Pop();
            if (!existingInstance.Any()) {
                pool.Remove(scene);
            }
            AddChild(instance);
            instance.RequestReady();

            return instance;
        } else {
            Node instance = scene.Instantiate();
            if (instance is IPoolable poolableInstance) {
                poolableInstance.Prepare();
            }
            AddChild(instance);

            return instance;
        }
    }

    public void ReturnInstance(Node instance, PackedScene scene) {
        if (instance.GetParent() != null) {
            instance.GetParent().CallDeferred("remove_child", instance);
        }

        if (pool.TryGetValue(scene, out var existingInstance)) {
            existingInstance.Append(instance);
        } else {
            Stack<Node> newStack = new();
            newStack.Push(instance);
            pool.Add(scene, newStack);
        }
    }
}

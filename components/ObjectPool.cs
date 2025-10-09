using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ObjectPool : Node {
    private Dictionary<PackedScene, Stack<Node>> instances;

    public Node RequestInstantiate(PackedScene scene) {
        if (instances.TryGetValue(scene, out var existingInstance)) {
            Node instance = existingInstance.Pop();
            if (!existingInstance.Any()) {
                instances.Remove(scene);
            }
            instance.RequestReady();

            return instance;
        } else {
            Node instance = scene.Instantiate();
            if (instance is IPoolable poolableInstance) {
                poolableInstance.Prepare();
            }
            
            return instance;
        }
    }

    public void ReturnInstance(Node instance, PackedScene scene) {
        if (instance.GetParent() != null) {
            instance.GetParent().RemoveChild(instance);
        }

        if (instances.TryGetValue(scene, out var existingInstance)) {
            existingInstance.Append(instance);
        } else {
            Stack<Node> newStack = new();
            newStack.Push(instance);
            instances.Add(scene, newStack);
        }
    }
}

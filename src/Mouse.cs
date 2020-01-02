using Godot;
using System;
using System.Linq;

public class Mouse : Spatial {

	private RigidBody mousePhysics;

	public override void _Ready() {
		mousePhysics = GetNode<RigidBody>("MousePhysics");
		mousePhysics.ContactMonitor = true;
		mousePhysics.ContactsReported = 10;

	}

	public override void _PhysicsProcess(float delta) {
		mousePhysics.GetCollidingBodies().Cast<Node>().Where(z => z.Name.Equals("CheesePhysics")).ToList().ForEach(z => z.QueueFree());
	}
}

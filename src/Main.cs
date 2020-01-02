// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();
	private PackedScene floorNmPS = GD.Load<PackedScene>("res://scenes/Floor-nm.tscn");
	private List<Vector3> pathPts = new List<Vector3>();
	private PackedScene cheesePS = GD.Load<PackedScene>("res://scenes/Cheese.tscn");
	private const float rayLength = 100;
	private Navigation floorNav;
	private int currPathNodeIndex = 0;

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

		floorNav = new Navigation();
		AddChild(floorNav);
		var floorNm = floorNmPS.Instance();
		floorNav.AddChild(floorNm);
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventScreenTouch st && st.Pressed) {
			var rayOrigin = GetNode<Camera>("cam").ProjectRayOrigin(st.Position);
			var rayDest = rayOrigin + GetNode<Camera>("cam").ProjectRayNormal(st.Position) * rayLength;
			var clickLoc = floorNav.GetClosestPointToSegment(rayOrigin, rayDest);
			var cheese = (Spatial)cheesePS.Instance();
			cheese.Translate(clickLoc);
			cheese.Scale *= 0.2f;
			AddChild(cheese);
			pathPts = floorNav.GetSimplePath(floorNav.GetClosestPoint(GetNode<Spatial>("Mouse").Translation), clickLoc, true).ToList();
			currPathNodeIndex = 0;

			GetNode<Spatial>("Mouse").LookAt(new Vector3(clickLoc.x, 0, clickLoc.z), new Vector3(0, 1, 0));
		}
	}

	public override void _Process(float delta) {
		if (pathPts.Any()) {
			if (currPathNodeIndex < pathPts.Count() - 1) {
				currPathNodeIndex++;

				var mouse = GetNode<Spatial>("Mouse");
				var tempTrans = mouse.Transform;
				tempTrans.origin = pathPts[currPathNodeIndex];
				mouse.Transform = tempTrans;
			}

		}
	}

}

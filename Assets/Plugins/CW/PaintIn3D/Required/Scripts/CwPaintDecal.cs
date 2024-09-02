﻿using UnityEngine;
using CW.Common;
using PaintCore;

namespace PaintIn3D
{
	/// <summary>This allows you to paint a decal at a hit point. Hit points will automatically be sent by any <b>CwHit___</b> component on this GameObject, or its ancestors.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwPaintDecal")]
	[AddComponentMenu(PaintCore.CwCommon.ComponentHitMenuPrefix + "Paint Decal")]
	public class CwPaintDecal : MonoBehaviour, IHitPoint, IHitLine, IHitTriangle, IHitQuad, IHitCoord
	{
		/// <summary>Only the CwPaintable___ GameObjects whose layers are within this mask will be eligible for painting.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>If this is set, then only the specified CwPaintable___ will be painted, regardless of the layer setting.</summary>
		public CwPaintableMeshAtlas TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private CwPaintableMeshAtlas targetModel;

		/// <summary>Only the <b>CwPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public CwGroup Group { set { group = value; } get { return group; } } [SerializeField] private CwGroup group;

		/// <summary>If this is set, then only the specified CwPaintableTexture will be painted, regardless of the layer or group setting.</summary>
		public CwPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private CwPaintableTexture targetTexture;

		/// <summary>This allows you to choose how the paint from this component will combine with the existing pixels of the textures you paint.
		/// NOTE: See the <b>Blend Mode</b> section of the documentation for more information.</summary>
		public CwBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private CwBlendMode blendMode = CwBlendMode.AlphaBlend(Vector4.one);

		/// <summary>The decal that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>This allows you to specify the shape of the decal. This is optional for most blending modes, because they usually derive their shape from the RGB or A values. However, if you're using the <b>Replace</b> blending mode, then you must manually specify the shape.</summary>
		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

		/// <summary>This allows you specify the texture channel used when sampling <b>Shape</b>.</summary>
		public CwChannel ShapeChannel { set { shapeChannel = value; } get { return shapeChannel; } } [SerializeField] private CwChannel shapeChannel = CwChannel.Alpha;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>The angle of the decal in degrees.</summary>
		public float Angle { set { angle = value; } get { return angle; } } [Range(-180.0f, 180.0f)] [SerializeField] private float angle;

		/// <summary>This allows you to control the mirroring and aspect ratio of the decal.
		/// 1, 1 = No scaling.
		/// -1, 1 = Horizontal Flip.</summary>
		public Vector3 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector3 scale = Vector3.one;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>This allows you to control the sharpness of the near+far depth cut-off point.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 3.0f;

		/// <summary>This allows you to control how much the decal can wrap around uneven paint surfaces.</summary>
		public float Wrapping { set { wrapping = value; } get { return wrapping; } } [SerializeField] [Range(0.0f, 1.0f)] private float wrapping = 1.0f;

		/// <summary>This allows you to control how much the paint can wrap around the front of surfaces.
		/// For example, if you want paint to wrap around curved surfaces then set this to a higher value.
		/// NOTE: If you set this to 0 then paint will not be applied to front facing surfaces.</summary>
		public float NormalFront { set { normalFront = value; } get { return normalFront; } } [Range(0.0f, 2.0f)] [SerializeField] private float normalFront = 1.0f;

		/// <summary>This works just like <b>Normal Front</b>, except for back facing surfaces.
		/// NOTE: If you set this to 0 then paint will not be applied to back facing surfaces.</summary>
		public float NormalBack { set { normalBack = value; } get { return normalBack; } } [Range(0.0f, 2.0f)] [SerializeField] private float normalBack;

		/// <summary>This allows you to control the smoothness of the normal cut-off point.</summary>
		public float NormalFade { set { normalFade = value; } get { return normalFade; } } [Range(0.001f, 0.5f)] [SerializeField] private float normalFade = 0.01f;

		/// <summary>This allows you to apply a tiled detail texture to your decals. This tiling will be applied in world space using triplanar mapping.</summary>
		public Texture TileTexture { set { tileTexture = value; } get { return tileTexture; } } [SerializeField] private Texture tileTexture;

		/// <summary>This allows you to adjust the tiling position + rotation + scale using a <b>Transform</b>.</summary>
		public Transform TileTransform { set { tileTransform = value; } get { return tileTransform; } } [SerializeField] private Transform tileTransform;

		/// <summary>This allows you to control the triplanar influence.
		/// 0 = No influence.
		/// 1 = Full influence.</summary>
		public float TileOpacity { set { tileOpacity = value; } get { return tileOpacity; } } [UnityEngine.Serialization.FormerlySerializedAs("tileBlend")] [Range(0.0f, 1.0f)] [SerializeField] private float tileOpacity = 1.0f;

		/// <summary>This allows you to control how quickly the triplanar mapping transitions between the X/Y/Z planes.</summary>
		public float TileTransition { set { tileTransition = value; } get { return tileTransition; } } [Range(1.0f, 200.0f)] [SerializeField] private float tileTransition = 4.0f;

		/// <summary>If your scene contains a <b>CwMask</b>, should this paint component use it?</summary>
		public bool FindMask { set { findMask = value; } get { return findMask; } } [SerializeField] private bool findMask = true;

		/// <summary>If your scene contains a <b>CwRenderDepth</b>, should this paint component use it?</summary>
		public bool FindDepthMask { set { findDepthMask = value; } get { return findDepthMask; } } [SerializeField] private bool findDepthMask = true;

		/// <summary>This stores a list of all modifiers used to change the way this component applies paint (e.g. <b>CwModifyColorRandom</b>).</summary>
		public CwModifierList Modifiers { get { if (modifiers == null) modifiers = new CwModifierList(); return modifiers; } } [SerializeField] private CwModifierList modifiers;

		/// <summary>This method will invert the scale.x value.</summary>
		[ContextMenu("Flip Horizontal")]
		public void FlipHorizontal()
		{
			scale.x = -scale.x;
		}

		/// <summary>This method will invert the scale.y value.</summary>
		[ContextMenu("Flip Vertical")]
		public void FlipVertical()
		{
			scale.y = -scale.y;
		}

		/// <summary>This method increments the angle by the specified amount of degrees, and wraps it to the -180..180 range.</summary>
		public void IncrementAngle(float degrees)
		{
			angle = Mathf.Repeat(angle + 180.0f + degrees, 360.0f) - 180.0f;
		}

		/// <summary>This method multiplies the <b>Opacity</b> by the specified value.</summary>
		public void MultiplyOpacity(float multiplier)
		{
			opacity = Mathf.Clamp01(opacity * multiplier);
		}

		/// <summary>This method increments the <b>Opacity</b> by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		/// <summary>This method multiplies the <b>Radius</b> by the specified value.</summary>
		public void MultiplyRadius(float multiplier)
		{
			radius *= multiplier;
		}

		/// <summary>This method increases the <b>Radius</b> by the specified value.</summary>
		public void IncrementRadius(float delta)
		{
			radius += delta;
		}

		/// <summary>This method multiplies the <b>Scale</b> by the specified value.</summary>
		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		/// <summary>This method increases the <b>Scale</b> by the specified value.</summary>
		public void IncrementScale(float multiplier)
		{
			scale += Vector3.one * multiplier;
		}

		/// <summary>This method paints all pixels at the specified point using the shape of a decal.</summary>
		public void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
		{
			if (modifiers != null && modifiers.Count > 0)
			{
				CwHelper.BeginSeed(seed);
					modifiers.ModifyPosition(ref position, preview, pressure);
				CwHelper.EndSeed();
			}

			CwCommandDecal.Instance.SetState(preview, priority);
			CwCommandDecal.Instance.SetLocation(position);

			var worldSize     = HandleHitCommon(preview, pressure, seed, rotation);
			var worldRadius   = PaintCore.CwCommon.GetRadius(worldSize);
			var worldPosition = position;

			HandleMaskCommon(worldPosition);

			CwPaintableManager.SubmitAll(CwCommandDecal.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		/// <summary>This method paints all pixels between the two specified points using the shape of a decal.</summary>
		public void HandleHitLine(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 endPosition, Quaternion rotation, bool clip)
		{
			CwCommandDecal.Instance.SetState(preview, priority);
			CwCommandDecal.Instance.SetLocation(position, endPosition, clip: clip);

			var worldSize     = HandleHitCommon(preview, pressure, seed, rotation);
			var worldRadius   = PaintCore.CwCommon.GetRadius(worldSize, position, endPosition);
			var worldPosition = PaintCore.CwCommon.GetPosition(position, endPosition);

			HandleMaskCommon(worldPosition);

			CwPaintableManager.SubmitAll(CwCommandDecal.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		/// <summary>This method paints all pixels between three points using the shape of a decal.</summary>
		public void HandleHitTriangle(bool preview, int priority, float pressure, int seed, Vector3 positionA, Vector3 positionB, Vector3 positionC, Quaternion rotation)
		{
			CwCommandDecal.Instance.SetState(preview, priority);
			CwCommandDecal.Instance.SetLocation(positionA, positionB, positionC);

			var worldSize     = HandleHitCommon(preview, pressure, seed, rotation);
			var worldRadius   = PaintCore.CwCommon.GetRadius(worldSize, positionA, positionB, positionC);
			var worldPosition = PaintCore.CwCommon.GetPosition(positionA, positionB, positionC);

			HandleMaskCommon(worldPosition);

			CwPaintableManager.SubmitAll(CwCommandDecal.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		/// <summary>This method paints all pixels between two pairs of points using the shape of a decal.</summary>
		public void HandleHitQuad(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 endPosition, Vector3 position2, Vector3 endPosition2, Quaternion rotation, bool clip)
		{
			CwCommandDecal.Instance.SetState(preview, priority);
			CwCommandDecal.Instance.SetLocation(position, endPosition, position2, endPosition2, clip: clip);

			var worldSize     = HandleHitCommon(preview, pressure, seed, rotation);
			var worldRadius   = PaintCore.CwCommon.GetRadius(worldSize, position, endPosition, position2, endPosition2);
			var worldPosition = PaintCore.CwCommon.GetPosition(position, endPosition, position2, endPosition2);

			HandleMaskCommon(worldPosition);

			CwPaintableManager.SubmitAll(CwCommandDecal.Instance, worldPosition, worldRadius, layers, group, targetModel, targetTexture);
		}

		/// <summary>This method paints the scene using the current component settings at the specified <b>CwHit</b>.</summary>
		public void HandleHitCoord(bool preview, int priority, float pressure, int seed, CwHit hit, Quaternion rotation)
		{
			var model = hit.Transform.GetComponent<CwPaintableMeshAtlas>();

			if (model != null)
			{
				var paintableTextures = model.FindPaintableTextures(group);

				for (var i = paintableTextures.Count - 1; i >= 0; i--)
				{
					var paintableTexture = paintableTextures[i];
					var coord            = paintableTexture.GetCoord(ref hit);

					if (modifiers != null && modifiers.Count > 0)
					{
						var position = (Vector3)coord;

						CwHelper.BeginSeed(seed);
							modifiers.ModifyPosition(ref position, preview, pressure);
						CwHelper.EndSeed();

						coord = position;
					}

					CwCommandDecal.Instance.SetState(preview, priority);
					CwCommandDecal.Instance.SetLocation(coord, false);

					HandleHitCommon(preview, pressure, seed, rotation);

					CwCommandDecal.Instance.ClearMask();

					CwCommandDecal.Instance.ApplyAspect(paintableTexture.Current);

					CwPaintableManager.Submit(CwCommandDecal.Instance, model, paintableTexture);
				}
			}
		}

		private Vector3 HandleHitCommon(bool preview, float pressure, int seed, Quaternion rotation)
		{
			var finalOpacity  = opacity;
			var finalRadius   = radius;
			var finalScale    = scale;
			var finalHardness = hardness;
			var finalColor    = color;
			var finalAngle    = angle;
			var finalTexture  = texture;
			var finalMatrix   = tileTransform != null ? tileTransform.localToWorldMatrix : Matrix4x4.identity;

			if (modifiers != null && modifiers.Count > 0)
			{
				CwHelper.BeginSeed(seed);
					modifiers.ModifyColor(ref finalColor, preview, pressure);
					modifiers.ModifyAngle(ref finalAngle, preview, pressure);
					modifiers.ModifyOpacity(ref finalOpacity, preview, pressure);
					modifiers.ModifyRadius(ref finalRadius, preview, pressure);
					modifiers.ModifyScale(ref finalScale, preview, pressure);
					modifiers.ModifyHardness(ref finalHardness, preview, pressure);
					modifiers.ModifyTexture(ref finalTexture, preview, pressure);
				CwHelper.EndSeed();
			}

			var finalAspect = PaintCore.CwCommon.GetAspect(shape, finalTexture);
			var finalSize   = PaintCore.CwCommon.ScaleAspect(finalScale * finalRadius, finalAspect);

			CwCommandDecal.Instance.SetShape(rotation, finalSize, finalAngle);

			CwCommandDecal.Instance.SetMaterial(blendMode, finalTexture, shape, shapeChannel, finalHardness, wrapping, normalBack, normalFront, normalFade, finalColor, finalOpacity, tileTexture, finalMatrix, tileOpacity, tileTransition);

			return finalSize;
		}

		private void HandleMaskCommon(Vector3 worldPosition)
		{
			if (findMask == true)
			{
				var mask = CwMask.Find(worldPosition, layers);

				if (mask != null)
				{
					CwCommandDecal.Instance.SetMask(mask.Matrix, mask.Texture, mask.Channel, mask.Invert, mask.Stretch);
				}
				else
				{
					CwCommandDecal.Instance.ClearMask();
				}
			}
			else
			{
				CwCommandDecal.Instance.ClearMask();
			}

			if (findDepthMask == true)
			{
				CwCommandDecal.Instance.DepthMask = CwRenderDepth.Find();
			}
			else
			{
				CwCommandDecal.Instance.DepthMask = null;
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

			Gizmos.DrawWireCube(Vector3.zero, scale * radius * 2.0f);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	using UnityEditor;
	using TARGET = CwPaintDecal;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwPaintDecal_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Layers == 0 && t.TargetModel == null));
				Draw("layers", "Only the CwPaintable___ GameObjects whose layers are within this mask will be eligible for painting.");
			EndError();
			Draw("group", "Only the CwPaintableTexture components with a matching group will be painted by this component.");

			Separator();

			Draw("blendMode", "This allows you to choose how the paint from this component will combine with the existing pixels of the textures you paint.\n\nNOTE: See the Blend Mode section of the documentation for more information.");
			BeginError(Any(tgts, t => t.Texture == null && t.Shape == null));
				Draw("texture", "The decal that will be painted.");
			EndError();
			EditorGUILayout.BeginHorizontal();
				BeginError(Any(tgts, t => t.BlendMode.Index == CwBlendMode.REPLACE && t.Shape == null));
					Draw("shape", "This allows you to specify the shape of the decal. This is optional for most blending modes, because they usually derive their shape from the RGB or A values. However, if you're using the Replace blending mode, then you must manually specify the shape.");
				EndError();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeChannel"), GUIContent.none, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
			Draw("color", "The color of the paint.");
			Draw("opacity", "The opacity of the brush.");

			Separator();

			Draw("angle", "The angle of the decal in degrees.");
			Draw("scale", "This allows you to control the mirroring and aspect ratio of the decal.\n\n1, 1 = No scaling.\n-1, 1 = Horizontal Flip.");
			BeginError(Any(tgts, t => t.Radius <= 0.0f));
				Draw("radius", "The radius of the paint brush.");
			EndError();
			BeginError(Any(tgts, t => t.Hardness <= 0.0f));
				Draw("hardness", "This allows you to control the sharpness of the near+far depth cut-off point.");
			EndError();
			Draw("wrapping", "This allows you to control how much the decal can wrap around uneven paint surfaces.");

			Separator();

			if (DrawFoldout("Advanced", "Show advanced settings?") == true)
			{
				BeginIndent();
					Draw("targetModel", "If this is set, then only the specified CwPaintable___ will be painted, regardless of the layer setting.");
					Draw("targetTexture", "If this is set, then only the specified CwPaintableTexture will be painted, regardless of the layer or group setting.");

					Separator();

					Draw("normalFront", "This allows you to control how much the paint can wrap around the front of surfaces (e.g. if you want paint to wrap around curved surfaces then set this to a higher value).\n\nNOTE: If you set this to 0 then paint will not be applied to front facing surfaces.");
					Draw("normalBack", "This works just like Normal Front, except for back facing surfaces.\n\nNOTE: If you set this to 0 then paint will not be applied to back facing surfaces.");
					Draw("normalFade", "This allows you to control the smoothness of the depth cut-off point.");

					Separator();

					Draw("tileTexture", "This allows you to apply a tiled detail texture to your decals. This tiling will be applied in world space using triplanar mapping.");
					Draw("tileTransform", "This allows you to adjust the tiling position + rotation + scale using a Transform.");
					Draw("tileOpacity", "This allows you to control the triplanar influence.\n\n0 = No influence.\n\n1 = Full influence.");
					Draw("tileTransition", "This allows you to control how quickly the triplanar mapping transitions between the X/Y/Z planes.");
					Draw("findMask", "If your scene contains a <b>CwMask</b>, should this paint component use it?");
					Draw("findDepthMask", "If your scene contains a <b>CwRenderDepth</b>, should this paint component use it?");
				EndIndent();
			}

			Separator();

			tgt.Modifiers.DrawEditorLayout(serializedObject, target, "Color", "Angle", "Opacity", "Radius", "Scale", "Hardness", "Texture", "Position");
		}
	}
}
#endif
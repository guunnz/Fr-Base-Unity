using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics.External.LibTessDotNet;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity.VectorGraphics
{
    public static partial class VectorUtils
    {
        /// <summary>Tessellates a Scene object into triangles.</summary>
        /// <param name="scene">The scene containing the hierarchy to tessellate</param>
        /// <param name="tessellationOptions">The tessellation options</param>
        /// <param name="nodeOpacities">If provided, the resulting node opacities</param>
        /// <returns>A list of tesselated geometry</returns>
        public static List<Geometry> TessellateScene(Scene scene, TessellationOptions tessellationOptions,
            Dictionary<SceneNode, float> nodeOpacities = null)
        {
            Profiler.BeginSample("TessellateVectorScene");

            VectorClip.ResetClip();
            var geoms = TessellateNodeHierarchyRecursive(scene.Root, tessellationOptions, scene.Root.Transform, 1.0f,
                nodeOpacities);

            Profiler.EndSample();

            return geoms;
        }

        private static void TessellateShape(Shape vectorShape, List<Geometry> geoms,
            TessellationOptions tessellationOptions, bool isConvex)
        {
            Profiler.BeginSample("TessellateShape");

            // Don't generate any geometry for pattern fills since these are generated from another SceneNode
            if (vectorShape.Fill != null && !(vectorShape.Fill is PatternFill))
            {
                var shapeColor = Color.white;
                if (vectorShape.Fill is SolidFill)
                    shapeColor = ((SolidFill) vectorShape.Fill).Color;

                shapeColor.a *= vectorShape.Fill.Opacity;

                if (isConvex && vectorShape.Contours.Length == 1)
                    TessellateConvexContour(vectorShape, vectorShape.PathProps.Stroke, shapeColor, geoms,
                        tessellationOptions);
                else
                    TessellateShapeLibTess(vectorShape, shapeColor, geoms, tessellationOptions);
            }

            var stroke = vectorShape.PathProps.Stroke;
            if (stroke != null && stroke.HalfThickness > Epsilon)
            {
                var strokeFill = stroke.Fill;
                var strokeColor = Color.white;
                if (strokeFill is SolidFill)
                {
                    strokeColor = ((SolidFill) strokeFill).Color;
                    strokeFill = null;
                }

                foreach (var c in vectorShape.Contours)
                {
                    Vector2[] strokeVerts;
                    ushort[] strokeIndices;
                    TessellatePath(c, vectorShape.PathProps, tessellationOptions, out strokeVerts, out strokeIndices);
                    if (strokeIndices.Length > 0)
                        geoms.Add(new Geometry
                        {
                            Vertices = strokeVerts, Indices = strokeIndices, Color = strokeColor, Fill = strokeFill,
                            FillTransform = stroke.FillTransform
                        });
                }
            }

            Profiler.EndSample();
        }

        private static void TessellateConvexContour(Shape shape, Stroke stroke, Color color, List<Geometry> geoms,
            TessellationOptions tessellationOptions)
        {
            if (shape.Contours.Length != 1 || shape.Contours[0].Segments.Length == 0)
                return;

            Profiler.BeginSample("TessellateConvexContour");

            // Compute geometric mean
            var contour = shape.Contours[0];
            var mean = Vector2.zero;
            foreach (var seg in contour.Segments)
                mean += seg.P0;
            mean /= contour.Segments.Length;

            // Trace the shape and build triangle fan
            var tracedShape = TraceShape(contour, stroke, tessellationOptions);
            var vertices = new Vector2[tracedShape.Length + 1];
            var indices = new ushort[tracedShape.Length * 3];

            vertices[0] = mean;
            for (var i = 0; i < tracedShape.Length; ++i)
            {
                vertices[i + 1] = tracedShape[i];
                indices[i * 3] = 0;
                indices[i * 3 + 1] = (ushort) (i + 1);
                indices[i * 3 + 2] = i + 2 >= vertices.Length ? (ushort) 1 : (ushort) (i + 2);
            }

            geoms.Add(new Geometry
            {
                Vertices = vertices, Indices = indices, Color = color, Fill = shape.Fill,
                FillTransform = shape.FillTransform
            });

            Profiler.EndSample();
        }

        private static void TessellateShapeLibTess(Shape vectorShape, Color color, List<Geometry> geoms,
            TessellationOptions tessellationOptions)
        {
            Profiler.BeginSample("LibTess");

            var tess = new Tess();

            var angle = 45.0f * Mathf.Deg2Rad;
            var mat = Matrix2D.RotateLH(angle);
            var invMat = Matrix2D.RotateLH(-angle);

            foreach (var c in vectorShape.Contours)
            {
                var contour = new List<Vector2>(100);
                foreach (var v in TraceShape(c, vectorShape.PathProps.Stroke, tessellationOptions))
                    contour.Add(mat.MultiplyPoint(v));

                tess.AddContour(
                    contour.Select(v => new ContourVertex {Position = new Vec3 {X = v.x, Y = v.y}}).ToArray(),
                    ContourOrientation.Original);
            }

            var windingRule = vectorShape.Fill.Mode == FillMode.OddEven ? WindingRule.EvenOdd : WindingRule.NonZero;
            try
            {
                tess.Tessellate(windingRule, ElementType.Polygons, 3);
            }
            catch (Exception)
            {
                Debug.LogWarning("Shape tessellation failed, skipping...");
                Profiler.EndSample();
                return;
            }

            var indices = tess.Elements.Select(i => (ushort) i);
            var vertices = tess.Vertices.Select(v => invMat.MultiplyPoint(new Vector2(v.Position.X, v.Position.Y)));

            if (indices.Count() > 0)
                geoms.Add(new Geometry
                {
                    Vertices = vertices.ToArray(), Indices = indices.ToArray(), Color = color, Fill = vectorShape.Fill,
                    FillTransform = vectorShape.FillTransform
                });

            Profiler.EndSample();
        }

        internal static Vector2[] GenerateShapeUVs(Vector2[] verts, Rect bounds, Matrix2D uvTransform)
        {
            Profiler.BeginSample("GenerateShapeUVs");

            uvTransform =
                Matrix2D.Translate(new Vector2(0, 1)) * Matrix2D.Scale(new Vector2(1.0f, -1.0f)) * // Do 1-uv.y
                uvTransform *
                Matrix2D.Scale(new Vector2(1.0f / bounds.width, 1.0f / bounds.height)) *
                Matrix2D.Translate(-bounds.position);
            var uvs = new Vector2[verts.Length];
            var vertCount = verts.Length;
            for (var i = 0; i < vertCount; i++)
                uvs[i] = uvTransform * verts[i];

            Profiler.EndSample();

            return uvs;
        }

        private static void SwapXY(ref Vector2 v)
        {
            var t = v.x;
            v.x = v.y;
            v.y = t;
        }

        /// <summary>
        ///     Generates a Texture2D atlas containing the textures and gradients for the vector geometry, and fill the UVs of
        ///     the geometry.
        /// </summary>
        /// <param name="geoms">The list of Geometry objects, probably created with TessellateNodeHierarchy</param>
        /// <param name="rasterSize">Maximum size of the generated texture</param>
        /// <returns>The generated texture atlas</returns>
        public static TextureAtlas GenerateAtlasAndFillUVs(IEnumerable<Geometry> geoms, uint rasterSize)
        {
            var atlas = GenerateAtlas(geoms, rasterSize);
            if (atlas != null)
                FillUVs(geoms, atlas);
            return atlas;
        }

        private static int NextPOT(int v)
        {
            if (v <= 0)
                return 0;
            --v;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            return ++v;
        }

        /// <summary>Generates a Texture2D atlas containing the textures and gradients for the vector geometry.</summary>
        /// <param name="geoms">The list of Geometry objects, probably created with TessellateNodeHierarchy</param>
        /// <param name="rasterSize">Maximum size of the generated texture</param>
        /// <param name="generatePOTTexture">Resize the texture to the next power-of-two</param>
        /// <param name="encodeSettings">Encode the gradient settings inside the texture</param>
        /// <returns>The generated texture atlas</returns>
        public static TextureAtlas GenerateAtlas(IEnumerable<Geometry> geoms, uint rasterSize,
            bool generatePOTTexture = true, bool encodeSettings = true)
        {
            var fills = new Dictionary<IFill, AtlasEntry>();
            var texturedGeomCount = 0;
            foreach (var g in geoms)
            {
                RawTexture tex;
                if (g.Fill is GradientFill)
                {
                    tex = new RawTexture
                    {
                        Width = (int) rasterSize, Height = 1,
                        Rgba = RasterizeGradientStripe((GradientFill) g.Fill, (int) rasterSize)
                    };
                    ++texturedGeomCount;
                }
                else if (g.Fill is TextureFill)
                {
                    var fillTex = ((TextureFill) g.Fill).Texture;
                    tex = new RawTexture {Rgba = fillTex.GetPixels32(), Width = fillTex.width, Height = fillTex.height};
                    ++texturedGeomCount;
                }
                else
                {
                    continue;
                }

                fills[g.Fill] = new AtlasEntry {Texture = tex};
            }

            if (fills.Count == 0)
                return null;

            Profiler.BeginSample("GenerateAtlas");

            Vector2 atlasSize;
            var rectsToPack = fills.Select(x =>
                    new KeyValuePair<IFill, Vector2>(x.Key, new Vector2(x.Value.Texture.Width, x.Value.Texture.Height)))
                .ToList();
            rectsToPack.Add(new KeyValuePair<IFill, Vector2>(null, new Vector2(2, 2))); // White fill
            var pack = PackRects(rectsToPack, out atlasSize);

            if (encodeSettings)
            {
                // The first row/cols of the atlas is reserved for the gradient settings
                for (var packIndex = 0; packIndex < pack.Count; ++packIndex)
                {
                    var item = pack[packIndex];
                    item.Position.x += 3;
                    pack[packIndex] = item;
                }

                atlasSize.x += 3;
            }

            // Need enough space on first 3 columns for texture settings
            var maxSettingIndex = 0;
            foreach (var item in pack)
                maxSettingIndex = Math.Max(maxSettingIndex, item.SettingIndex);
            var minWidth = encodeSettings ? 3 : 0;
            var minHeight = encodeSettings ? maxSettingIndex + 1 : maxSettingIndex;
            atlasSize.x = Math.Max(minWidth, (int) atlasSize.x);
            atlasSize.y = Math.Max(minHeight, (int) atlasSize.y);

            var atlasWidth = (int) atlasSize.x;
            var atlasHeight = (int) atlasSize.y;
            if (generatePOTTexture)
            {
                atlasWidth = NextPOT(atlasWidth);
                atlasHeight = NextPOT(atlasHeight);
            }

            var atlasColors = new Color32[atlasWidth * atlasHeight];
            for (var k = 0; k < atlasWidth * atlasHeight; ++k)
                atlasColors[k] = Color.black;
            var atlasInvSize = new Vector2(1.0f / atlasWidth, 1.0f / atlasHeight);
            var whiteTexelsScreenPos = pack[pack.Count - 1].Position;

            var i = 0;
            var rawAtlasTex = new RawTexture {Rgba = atlasColors, Width = atlasWidth, Height = atlasHeight};
            foreach (var entry in fills.Values)
            {
                var packItem = pack[i++];
                entry.AtlasLocation = packItem;
                BlitRawTexture(entry.Texture, rawAtlasTex, (int) packItem.Position.x, (int) packItem.Position.y,
                    packItem.Rotated);
            }

            var whiteTex = new RawTexture {Width = 2, Height = 2, Rgba = new Color32[4]};
            for (i = 0; i < whiteTex.Rgba.Length; i++)
                whiteTex.Rgba[i] = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            BlitRawTexture(whiteTex, rawAtlasTex, (int) whiteTexelsScreenPos.x, (int) whiteTexelsScreenPos.y, false);

            if (encodeSettings)
                EncodeSettings(geoms, fills, rawAtlasTex, whiteTexelsScreenPos);

            var atlasTex = new Texture2D(atlasWidth, atlasHeight, TextureFormat.ARGB32, false, true);
            atlasTex.wrapModeU = TextureWrapMode.Clamp;
            atlasTex.wrapModeV = TextureWrapMode.Clamp;
            atlasTex.wrapModeW = TextureWrapMode.Clamp;
            atlasTex.SetPixels32(atlasColors);
            atlasTex.Apply(false, true);

            Profiler.EndSample();

            return new TextureAtlas {Texture = atlasTex, Entries = pack};
        }

        private static void EncodeSettings(IEnumerable<Geometry> geoms, Dictionary<IFill, AtlasEntry> fills,
            RawTexture rawAtlasTex, Vector2 whiteTexelsScreenPos)
        {
            // Setting 0 is reserved for the white texel
            WriteRawFloat4Packed(rawAtlasTex, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0);
            WriteRawInt2Packed(rawAtlasTex, (int) whiteTexelsScreenPos.x + 1, (int) whiteTexelsScreenPos.y + 1, 1, 0);
            WriteRawInt2Packed(rawAtlasTex, 0, 0, 2, 0);

            var writtenSettings = new HashSet<int>();
            writtenSettings.Add(0);

            foreach (var g in geoms)
            {
                AtlasEntry entry;
                var vertsCount = g.Vertices.Length;
                if (g.Fill != null && fills.TryGetValue(g.Fill, out entry))
                {
                    var setting = entry.AtlasLocation.SettingIndex;
                    if (writtenSettings.Contains(setting))
                        continue;

                    writtenSettings.Add(setting);

                    // There are 3 consecutive pixels to store the settings
                    var destX = 0;
                    var destY = setting;

                    var gradientFill = g.Fill as GradientFill;
                    if (gradientFill != null)
                    {
                        var focus = gradientFill.RadialFocus;
                        focus += Vector2.one;
                        focus /= 2.0f;
                        focus.y = 1.0f - focus.y;

                        WriteRawFloat4Packed(rawAtlasTex, (float) gradientFill.Type / 255,
                            (float) gradientFill.Addressing / 255, focus.x, focus.y, destX++, destY);
                    }

                    var textureFill = g.Fill as TextureFill;
                    if (textureFill != null)
                        WriteRawFloat4Packed(rawAtlasTex, 0.0f, (float) textureFill.Addressing / 255, 0.0f, 0.0f,
                            destX++, destY);

                    var pos = entry.AtlasLocation.Position;
                    var size = new Vector2(entry.Texture.Width - 1, entry.Texture.Height - 1);
                    WriteRawInt2Packed(rawAtlasTex, (int) pos.x, (int) pos.y, destX++, destY);
                    WriteRawInt2Packed(rawAtlasTex, (int) size.x, (int) size.y, destX++, destY);
                }
            }
        }

        /// <summary>Fill the UVs of the geometry using the provided texture atlas.</summary>
        /// <param name="geoms">The geometry that will have its UVs filled</param>
        /// <param name="texAtlas">The texture atlas used for the UV generation</param>
        public static void FillUVs(IEnumerable<Geometry> geoms, TextureAtlas texAtlas)
        {
            Profiler.BeginSample("FillUVs");

            var fills = new Dictionary<IFill, PackRectItem>();
            foreach (var entry in texAtlas.Entries)
                if (entry.Fill != null)
                    fills[entry.Fill] = entry;

            var item = new PackRectItem();
            foreach (var g in geoms)
            {
                var settingIndex = 0;
                if (g.Fill != null && fills.TryGetValue(g.Fill, out item))
                    settingIndex = item.SettingIndex;

                g.UVs = GenerateShapeUVs(g.Vertices, g.UnclippedBounds, g.FillTransform);
                g.SettingIndex = settingIndex;
            }

            Profiler.EndSample();
        }

        /// <summary>Holds the tessellated Scene geometry and associated data.</summary>
        public class Geometry
        {
            /// <summary>The color of the geometry.</summary>
            public Color Color;

            /// <summary>The fill of the geometry. May be null.</summary>
            public IFill Fill;

            /// <summary>The filling transform of the geometry.</summary>
            public Matrix2D FillTransform;

            /// <summary>The triangle indices of the geometry.</summary>
            public ushort[] Indices;

            /// <summary>The setting index of the geometry.</summary>
            /// <remarks>
            ///     This is used to refer to the proper texture/gradient settings inside the texture atlas.
            ///     This should be set to 0 for geometries without texture or gradients.
            /// </remarks>
            public int SettingIndex;

            /// <summary>The unclipped bounds of the geometry.</summary>
            public Rect UnclippedBounds;

            /// <summary>The UV coordinates of the geometry.</summary>
            public Vector2[] UVs;

            /// <summary>The vertices of the geometry.</summary>
            public Vector2[] Vertices;

            /// <summary>The world transform of the geometry.</summary>
            public Matrix2D WorldTransform;
        }

        internal struct RawTexture
        {
            public Color32[] Rgba;
            public int Width;
            public int Height;
        }

        private class AtlasEntry
        {
            public PackRectItem AtlasLocation;
            public RawTexture Texture;
        }

        /// <summary>A struct to hold packed atlas entries.</summary>
        public class TextureAtlas
        {
            /// <summary>The texture atlas.</summary>
            public Texture2D Texture { get; set; }

            /// <summary>The atlas entries.</summary>
            public List<PackRectItem> Entries { get; set; }
        }

#pragma warning disable 612, 618 // Silence use of deprecated IDrawable
        private static List<Geometry> TessellateNodeHierarchyRecursive(SceneNode node,
            TessellationOptions tessellationOptions, Matrix2D worldTransform, float worldOpacity,
            Dictionary<SceneNode, float> nodeOpacities)
        {
            if (node.Clipper != null)
                VectorClip.PushClip(TraceNodeHierarchyShapes(node.Clipper, tessellationOptions), worldTransform);

            var geoms = new List<Geometry>();

            if (node.Shapes != null)
                foreach (var shape in node.Shapes)
                {
                    var isConvex = shape.IsConvex && shape.Contours.Length == 1;
                    TessellateShape(shape, geoms, tessellationOptions, isConvex);
                }

            foreach (var g in geoms)
            {
                g.Color.a *= worldOpacity;
                g.WorldTransform = worldTransform;
                g.UnclippedBounds = Bounds(g.Vertices);

                VectorClip.ClipGeometry(g);
            }

            if (node.Children != null)
                foreach (var child in node.Children)
                {
                    var childOpacity = 1.0f;
                    if (nodeOpacities == null || !nodeOpacities.TryGetValue(child, out childOpacity))
                        childOpacity = 1.0f;

                    var transform = worldTransform * child.Transform;
                    var opacity = worldOpacity * childOpacity;
                    var childGeoms = TessellateNodeHierarchyRecursive(child, tessellationOptions, transform, opacity,
                        nodeOpacities);

                    geoms.AddRange(childGeoms);
                }

            if (node.Clipper != null)
                VectorClip.PopClip();

            return geoms;
        }

        internal static List<Vector2[]> TraceNodeHierarchyShapes(SceneNode root,
            TessellationOptions tessellationOptions)
        {
            var shapes = new List<Vector2[]>();

            foreach (var nodeInfo in WorldTransformedSceneNodes(root, null))
            {
                var node = nodeInfo.Node;

                if (node.Shapes != null)
                    foreach (var shape in node.Shapes)
                    foreach (var c in shape.Contours)
                    {
                        var tracedShape = TraceShape(c, shape.PathProps.Stroke, tessellationOptions);
                        if (tracedShape.Length > 0)
                            shapes.Add(tracedShape.Select(v => nodeInfo.WorldTransform * v).ToArray());
                    }
            }

            return shapes;
        }
#pragma warning restore 612, 618
    }
}
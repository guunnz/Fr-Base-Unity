using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics.External.ClipperLib;
using Unity.VectorGraphics.External.LibTessDotNet;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity.VectorGraphics
{
    internal static class VectorClip
    {
        private const int k_ClipperScale = 100000;

        private static readonly Stack<List<List<IntPoint>>> m_ClipStack = new Stack<List<List<IntPoint>>>();

        internal static void ResetClip()
        {
            m_ClipStack.Clear();
        }

        internal static void PushClip(List<Vector2[]> clipper, Matrix2D transform)
        {
            var clipperPaths = new List<List<IntPoint>>(10);
            foreach (var shape in clipper)
            {
                var verts = new List<IntPoint>(shape.Length);
                foreach (var v in shape)
                {
                    var tv = transform * v;
                    verts.Add(new IntPoint(tv.x * k_ClipperScale, tv.y * k_ClipperScale));
                }

                clipperPaths.Add(verts);
            }

            m_ClipStack.Push(clipperPaths);
        }

        internal static void PopClip()
        {
            m_ClipStack.Pop();
        }

        internal static void ClipGeometry(VectorUtils.Geometry geom)
        {
            Profiler.BeginSample("ClipGeometry");

            var clipper = new Clipper();
            foreach (var clipperPaths in m_ClipStack)
            {
                var vertices = new List<Vector2>(geom.Vertices.Length);
                var indices = new List<ushort>(geom.Indices.Length);
                var paths = BuildTriangleClipPaths(geom);
                var result = new List<List<IntPoint>>();

                ushort maxIndex = 0;

                foreach (var path in paths)
                {
                    clipper.AddPaths(clipperPaths, PolyType.ptClip, true);
                    clipper.AddPath(path, PolyType.ptSubject, true);
                    clipper.Execute(ClipType.ctIntersection, result, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

                    if (result.Count > 0)
                        BuildGeometryFromClipPaths(geom, result, vertices, indices, ref maxIndex);

                    clipper.Clear();
                    result.Clear();
                }

                geom.Vertices = vertices.ToArray();
                geom.Indices = indices.ToArray();
            }

            Profiler.EndSample();
        }

        private static List<List<IntPoint>> BuildTriangleClipPaths(VectorUtils.Geometry geom)
        {
            var paths = new List<List<IntPoint>>(geom.Indices.Length / 3);
            var verts = geom.Vertices;
            var inds = geom.Indices;
            var indexCount = geom.Indices.Length;
            var matrix = geom.WorldTransform;
            for (var i = 0; i < indexCount; i += 3)
            {
                var v0 = matrix * verts[inds[i]];
                var v1 = matrix * verts[inds[i + 1]];
                var v2 = matrix * verts[inds[i + 2]];
                var tri = new List<IntPoint>(3);
                tri.Add(new IntPoint(v0.x * k_ClipperScale, v0.y * k_ClipperScale));
                tri.Add(new IntPoint(v1.x * k_ClipperScale, v1.y * k_ClipperScale));
                tri.Add(new IntPoint(v2.x * k_ClipperScale, v2.y * k_ClipperScale));
                paths.Add(tri);
            }

            return paths;
        }

        private static void BuildGeometryFromClipPaths(VectorUtils.Geometry geom, List<List<IntPoint>> paths,
            List<Vector2> outVerts, List<ushort> outInds, ref ushort maxIndex)
        {
            var vertices = new List<Vector2>(100);
            var indices = new List<ushort>(vertices.Capacity * 3);
            var vertexIndex = new Dictionary<IntPoint, ushort>();

            foreach (var path in paths)
                if (path.Count == 3)
                {
                    // Triangle case, no need to tessellate
                    foreach (var pt in path)
                        StoreClipVertex(vertexIndex, vertices, indices, pt, ref maxIndex);
                }
                else if (path.Count > 3)
                {
                    // Generic polygon case, we need to tessellate first
                    var tess = new Tess();
                    var contour = new ContourVertex[path.Count];
                    for (var i = 0; i < path.Count; ++i)
                        contour[i] = new ContourVertex {Position = new Vec3 {X = path[i].X, Y = path[i].Y, Z = 0.0f}};
                    tess.AddContour(contour, ContourOrientation.Original);

                    var windingRule = WindingRule.NonZero;
                    tess.Tessellate(windingRule, ElementType.Polygons, 3);

                    foreach (var e in tess.Elements)
                    {
                        var v = tess.Vertices[e];
                        var pt = new IntPoint(v.Position.X, v.Position.Y);
                        StoreClipVertex(vertexIndex, vertices, indices, pt, ref maxIndex);
                    }
                }

            var invMatrix = geom.WorldTransform.Inverse();

            outVerts.AddRange(vertices.Select(v => invMatrix * v));
            outInds.AddRange(indices);
        }

        private static void StoreClipVertex(Dictionary<IntPoint, ushort> vertexIndex, List<Vector2> vertices,
            List<ushort> indices, IntPoint pt, ref ushort index)
        {
            ushort storedIndex;
            if (vertexIndex.TryGetValue(pt, out storedIndex))
            {
                indices.Add(storedIndex);
            }
            else
            {
                vertices.Add(new Vector2((float) pt.X / k_ClipperScale, (float) pt.Y / k_ClipperScale));
                indices.Add(index);
                vertexIndex[pt] = index;
                ++index;
            }
        }
    }
}
﻿// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OsmSharp;
using OsmSharp.Geo;
using OsmSharp.Streams;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.GeometryStream.Shape
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // let's show you what's going on.
            OsmSharp.Logging.Logger.LogAction = (origin, level, message, parameters) =>
            {
                Console.WriteLine($"[{origin}] {level} - {message}");
            };

            await Download.Download.ToFile("http://planet.anyways.eu/planet/europe/luxembourg/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf");
            
            await using var fileStream = File.OpenRead("luxembourg-latest.osm.pbf");
            // create source stream.
            var source = new PBFOsmStreamSource(fileStream);

            // show progress.
            var progress = source.ShowProgress();

            // filter all power lines and keep all nodes.
            var filtered = from osmGeo in progress
                where osmGeo.Type == OsmSharp.OsmGeoType.Node ||
                      (osmGeo.Type == OsmSharp.OsmGeoType.Way && osmGeo.Tags != null && osmGeo.Tags.Contains("power", "line"))
                select osmGeo;

            // convert to a feature stream.
            // WARNING: nodes that are part of power lines will be kept in-memory.
                //          it's important to filter only the objects you need **before** 
                //          you convert to a feature stream otherwise all objects will 
                //          be kept in-memory.
                var features = filtered.ToFeatureSource();

                // filter out only linestrings.
                var lineStrings = from feature in features
                                  where feature.Geometry is LineString
                                  select feature;

                // build feature collection.
                var featureCollection = new FeatureCollection();
                var attributesTable = new AttributesTable {{"type", "powerline"}};
                foreach (var feature in lineStrings)
                { // make sure there is a constant # of attributes with the same names before writing the shapefile.
                    featureCollection.Add(new Feature(feature.Geometry, attributesTable));
                }

            // convert to shape.
            var header = ShapefileDataWriter.GetHeader(featureCollection.First(), featureCollection.Count);
            var shapeWriter = new ShapefileDataWriter("luxembourg.shp", new GeometryFactory())
            {
                Header = header
            };
            shapeWriter.Write(featureCollection);
        }
    }
}
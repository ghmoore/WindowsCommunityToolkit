// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI.Controls.Lottie.Model.Animatable;
using Microsoft.Toolkit.Uwp.UI.Controls.Lottie.Model.Content;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Lottie.Parser
{
    internal static class GradientStrokeParser
    {
        internal static GradientStroke Parse(JsonReader reader, LottieComposition composition)
        {
            string name = null;
            AnimatableGradientColorValue color = null;
            AnimatableIntegerValue opacity = null;
            GradientType gradientType = GradientType.Linear;
            AnimatablePointValue startPoint = null;
            AnimatablePointValue endPoint = null;
            AnimatableFloatValue width = null;
            ShapeStroke.LineCapType capType = ShapeStroke.LineCapType.Unknown;
            ShapeStroke.LineJoinType joinType = ShapeStroke.LineJoinType.Round;
            AnimatableFloatValue offset = null;

            List<AnimatableFloatValue> lineDashPattern = new List<AnimatableFloatValue>();

            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "nm":
                        name = reader.NextString();
                        break;
                    case "g":
                        int points = -1;
                        reader.BeginObject();
                        while (reader.HasNext())
                        {
                            switch (reader.NextName())
                            {
                                case "p":
                                    points = reader.NextInt();
                                    break;
                                case "k":
                                    color = AnimatableValueParser.ParseGradientColor(reader, composition, points);
                                    break;
                                default:
                                    reader.SkipValue();
                                    break;
                            }
                        }

                        reader.EndObject();
                        break;
                    case "o":
                        opacity = AnimatableValueParser.ParseInteger(reader, composition);
                        break;
                    case "t":
                        gradientType = reader.NextInt() == 1 ? GradientType.Linear : GradientType.Radial;
                        break;
                    case "s":
                        startPoint = AnimatableValueParser.ParsePoint(reader, composition);
                        break;
                    case "e":
                        endPoint = AnimatableValueParser.ParsePoint(reader, composition);
                        break;
                    case "w":
                        width = AnimatableValueParser.ParseFloat(reader, composition);
                        break;
                    case "lc":
                        capType = (ShapeStroke.LineCapType)(reader.NextInt() - 1);
                        break;
                    case "lj":
                        joinType = (ShapeStroke.LineJoinType)(reader.NextInt() - 1);
                        break;
                    case "d":
                        reader.BeginArray();
                        while (reader.HasNext())
                        {
                            string n = null;
                            AnimatableFloatValue val = null;
                            reader.BeginObject();
                            while (reader.HasNext())
                            {
                                switch (reader.NextName())
                                {
                                    case "n":
                                        n = reader.NextString();
                                        break;
                                    case "v":
                                        val = AnimatableValueParser.ParseFloat(reader, composition);
                                        break;
                                    default:
                                        reader.SkipValue();
                                        break;
                                }
                            }

                            reader.EndObject();

                            if (n.Equals("o"))
                            {
                                offset = val;
                            }
                            else if (n.Equals("d") || n.Equals("g"))
                            {
                                lineDashPattern.Add(val);
                            }
                        }

                        reader.EndArray();
                        if (lineDashPattern.Count == 1)
                        {
                            // If there is only 1 value then it is assumed to be equal parts on and off.
                            lineDashPattern.Add(lineDashPattern[0]);
                        }

                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            return new GradientStroke(name, gradientType, color, opacity, startPoint, endPoint, width, capType, joinType, lineDashPattern, offset);
        }
    }
}
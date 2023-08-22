/*
 * Copyright (c) 2023 Sony Semiconductor Solutions Corporation
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/
// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace SmartCamera
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct ObjectDetectionData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_1_21(); }
  public static ObjectDetectionData GetRootAsObjectDetectionData(ByteBuffer _bb) { return GetRootAsObjectDetectionData(_bb, new ObjectDetectionData()); }
  public static ObjectDetectionData GetRootAsObjectDetectionData(ByteBuffer _bb, ObjectDetectionData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public ObjectDetectionData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public SmartCamera.GeneralObject? ObjectDetectionList(int j) { int o = __p.__offset(4); return o != 0 ? (SmartCamera.GeneralObject?)(new SmartCamera.GeneralObject()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int ObjectDetectionListLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<SmartCamera.ObjectDetectionData> CreateObjectDetectionData(FlatBufferBuilder builder,
      VectorOffset object_detection_listOffset = default(VectorOffset)) {
    builder.StartTable(1);
    ObjectDetectionData.AddObjectDetectionList(builder, object_detection_listOffset);
    return ObjectDetectionData.EndObjectDetectionData(builder);
  }

  public static void StartObjectDetectionData(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddObjectDetectionList(FlatBufferBuilder builder, VectorOffset objectDetectionListOffset) { builder.AddOffset(0, objectDetectionListOffset.Value, 0); }
  public static VectorOffset CreateObjectDetectionListVector(FlatBufferBuilder builder, Offset<SmartCamera.GeneralObject>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateObjectDetectionListVectorBlock(FlatBufferBuilder builder, Offset<SmartCamera.GeneralObject>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateObjectDetectionListVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<SmartCamera.GeneralObject>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateObjectDetectionListVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<SmartCamera.GeneralObject>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartObjectDetectionListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<SmartCamera.ObjectDetectionData> EndObjectDetectionData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<SmartCamera.ObjectDetectionData>(o);
  }
}


}

﻿using System;
using System.Drawing;
using System.IO;
using System.Collections;

/// <summary>
/// Description of ImageConverter.
/// </summary>
public class ImageConverter {
    public ImageConverter() {
    }

    public byte[] imageToByteArray(System.Drawing.Image image) {
        MemoryStream ms = new MemoryStream();
        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        return ms.ToArray();
    }

 
}
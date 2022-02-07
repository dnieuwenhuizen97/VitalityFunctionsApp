﻿using Infrastructure.StorageAccount;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class BlobStorageServiceTests
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly Mock<IBlobStorage> _blobDbMock = new Mock<IBlobStorage>();

        public BlobStorageServiceTests()
        {
            _blobStorageService = new BlobStorageService(_blobDbMock.Object);
        }

        [Fact]
        public async Task Get_Image_Should_Return_Image_Url_String()
        {
            // Arrange
            string imageName = "imagename";
            string imageUrl = "https://blob.com.picture.png";

            _blobDbMock.Setup(x => x.GetImage(imageName))
                .Returns(Task.FromResult(imageUrl));

            // Act
            string url = await _blobStorageService.GetImage(imageName);

            // Assert
            Assert.NotNull(url);
            Assert.Equal(imageUrl, url);
        }

        [Fact]
        public async Task Get_Image_Should_Return_Null_When_Image_Not_Found()
        {
            // Arrange
            string imageName = "imagename";
            string imageUrl = null;

            _blobDbMock.Setup(x => x.GetImage(imageName))
                .Returns(Task.FromResult(imageUrl));

            // Act
            string url = await _blobStorageService.GetImage(imageName);

            // Assert
            Assert.Null(url);
        }

        [Fact]
        public async Task Get_Video_Should_Return_Image_Url_String()
        {
            // Arrange
            string videoName = "videoname";
            string videoUrl = "https://blob.com.picture.png";

            _blobDbMock.Setup(x => x.GetVideo(videoName))
                .Returns(Task.FromResult(videoUrl));

            // Act
            string url = await _blobStorageService.GetVideo(videoName);

            // Assert
            Assert.NotNull(url);
            Assert.Equal(videoUrl, url);
        }

        [Fact]
        public async Task Get_Video_Should_Return_Null_When_Image_Not_Found()
        {
            // Arrange
            string videoName = "videoname";
            string videoUrl = null;

            _blobDbMock.Setup(x => x.GetVideo(videoName))
                .Returns(Task.FromResult(videoUrl));

            // Act
            string url = await _blobStorageService.GetVideo(videoName);

            // Assert
            Assert.Null(url);
        }
    }
}

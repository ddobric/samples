using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace PiCam
{
    public class PiCam
    {
        private MediaCapture mediaCapture;
        private StorageFile photoFile;
        private StorageFile recordStorageFile;
        private StorageFile audioFile;
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        private readonly string VIDEO_FILE_NAME = "video.mp4";
        private readonly string AUDIO_FILE_NAME = "audio.mp3";

        private bool isPreviewing;
        private bool isRecording;

        private Image captureImage;
        private CaptureElement previewElement;
        private MediaElement playbackElement;


        public PiCam(Image image, CaptureElement captureElement = null, MediaElement mediaElement = null)
        {
            captureImage = image;
            previewElement = captureElement;
            playbackElement = mediaElement;
        }


        public async Task Init()
        {
            try
            {
                if (mediaCapture != null)
                {
                    // Cleanup MediaCapture object
                    if (isPreviewing)
                    {
                        await mediaCapture.StopPreviewAsync();
                        captureImage.Source = null;
                        playbackElement.Source = null;
                        isPreviewing = false;
                    }
                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        isRecording = false;
                    }
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                // Use default initialization
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                // Set callbacks for failure and recording limit exceeded

                mediaCapture.Failed += MediaCapture_Failed;
                mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
                // Start Preview                
                previewElement.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async void InitVideo()
        {
            try
            {
                if (mediaCapture != null)
                {
                    // Cleanup MediaCapture object
                    if (isPreviewing)
                    {
                        await mediaCapture.StopPreviewAsync();
                        captureImage.Source = null;
                        playbackElement.Source = null;
                        isPreviewing = false;
                    }
                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        isRecording = false;
                    }
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                // Use default initialization
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                // Set callbacks for failure and recording limit exceeded

                mediaCapture.Failed += new MediaCaptureFailedEventHandler(MediaCapture_Failed);
                mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;

                // Start Preview                
                previewElement.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task TakePhoto()
        {
            try
            {
                captureImage.Source = null;

                photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                    PHOTO_FILE_NAME, CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

                IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(photoStream);
                captureImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                CloseCam();
            }
            finally
            {

            }
        }

        bool isVodeoRecording = false;

        public async void StartVideoRecording()
        {
            try
            {
                playbackElement.Source = null;

                String fileName;
                fileName = VIDEO_FILE_NAME;

                recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);


                MediaEncodingProfile recordProfile = null;
                recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);

                await mediaCapture.StartRecordToStorageFileAsync(recordProfile, recordStorageFile);

                isRecording = true;
            }
            catch (Exception ex)
            {
                if (ex is System.UnauthorizedAccessException)
                {
                    throw ex;
                }
                else
                {
                    CloseCam();
                }
            }
            finally
            {

            }
        }


        public async void StopVideoRecording()
        {
            try
            {

                playbackElement.Source = null;



                await mediaCapture.StopRecordAsync();
                isRecording = false;

                var stream = await recordStorageFile.OpenReadAsync();
                playbackElement.AutoPlay = true;
                playbackElement.SetSource(stream, recordStorageFile.FileType);
                playbackElement.Play();


            }
            catch (Exception ex)
            {
                if (ex is System.UnauthorizedAccessException)
                {

                }
                else
                {
                    CloseCam();
                }
            }
            finally
            {

            }
        }


        private void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            throw new NotImplementedException();
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {

            try
            {

                if (isRecording)
                {
                    mediaCapture.StopRecordAsync().AsTask().Wait();
                }
            }
            catch (Exception)
            {
            }
            finally
            {

            }

        }

        public async void CloseCam()
        {
            if (mediaCapture != null)
            {
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                    captureImage.Source = null;
                }
                if (isRecording)
                {
                    await mediaCapture.StopRecordAsync();
                    isRecording = false;
                }
                mediaCapture.Dispose();
                mediaCapture = null;
            }
        }


    }
}

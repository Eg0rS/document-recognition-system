package minio

import (
	"context"
	"fmt"
	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
	"github.com/theartofdevel/notes_system/file_service/pkg/logging"
	"io"
	"time"
)

type Object struct {
	ID   string
	Size int64
	Tags map[string]string
}

type Client struct {
	logger      logging.Logger
	minioClient *minio.Client
	bucket      string
}

func NewClient(endpoint, accessKeyID, secretAccessKey, bucket string, logger logging.Logger) (*Client, error) {
	minioClient, err := minio.New(endpoint, &minio.Options{
		Creds:  credentials.NewStaticV4(accessKeyID, secretAccessKey, ""),
		Secure: false,
	})
	if err != nil {
		return nil, fmt.Errorf("failed to create minio client. err: %w", err)
	}

	return &Client{
		logger:      logger,
		minioClient: minioClient,
		bucket:      bucket,
	}, nil
}

func (c *Client) GetFile(ctx context.Context, fileId string) (*minio.Object, error) {
	object, err := c.minioClient.GetObject(ctx, c.bucket, fileId, minio.GetObjectOptions{})
	if err != nil {
		c.logger.Errorf("failed to get object key=%s from minio bucket %s. err: %v", fileId, c.bucket, err)
		return nil, nil
	}

	return object, nil
}

func (c *Client) GetBucketFiles(ctx context.Context) ([]*minio.Object, error) {
	reqCtx, cancel := context.WithTimeout(ctx, 10*time.Second)
	defer cancel()

	var files []*minio.Object
	for lobj := range c.minioClient.ListObjects(reqCtx, c.bucket, minio.ListObjectsOptions{WithMetadata: true}) {
		if lobj.Err != nil {
			c.logger.Errorf("failed to list object from minio bucket %s. err: %v", c.bucket, lobj.Err)
			continue
		}
		object, err := c.minioClient.GetObject(ctx, c.bucket, lobj.Key, minio.GetObjectOptions{})
		if err != nil {
			c.logger.Errorf("failed to get object key=%s from minio bucket %s. err: %v", lobj.Key, c.bucket, lobj.Err)
			continue
		}
		files = append(files, object)
	}
	return files, nil
}

func (c *Client) UploadFile(ctx context.Context, fileId, fileName string, fileSize int64, reader io.Reader) error {
	reqCtx, cancel := context.WithTimeout(ctx, 10*time.Second)
	defer cancel()

	exists, errBucketExists := c.minioClient.BucketExists(ctx, c.bucket)
	var list, _ = c.minioClient.ListBuckets(ctx)
	if list == nil {
	}
	if errBucketExists != nil || !exists {
		c.logger.Warnf("no bucket %s. creating new one...", c.bucket)
		err := c.minioClient.MakeBucket(ctx, c.bucket, minio.MakeBucketOptions{})
		if err != nil {
			return fmt.Errorf("failed to create new bucket. err: %w", err)
		}
	}

	c.logger.Debugf("put new object %s to bucket %s", fileName, c.bucket)
	info, err := c.minioClient.PutObject(reqCtx, c.bucket, fileId, reader, fileSize,
		minio.PutObjectOptions{
			UserMetadata: map[string]string{
				"Name": fileName,
			},
			ContentType: "application/octet-stream",
		})
	if err != nil {
		return fmt.Errorf("failed to upload file. err: %w", err)
	}
	fmt.Println(info)
	return nil
}

func (c *Client) DeleteFile(ctx context.Context, fileName string) error {
	err := c.minioClient.RemoveObject(ctx, c.bucket, fileName, minio.RemoveObjectOptions{})
	if err != nil {
		return fmt.Errorf("failed to delete file. err: %w", err)
	}
	return nil
}

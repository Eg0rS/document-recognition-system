package file

import (
	"context"
)

type Storage interface {
	GetFile(ctx context.Context, fileName string) (*File, error)
	GetFilesByNoteUUID(ctx context.Context) ([]*File, error)
	CreateFile(ctx context.Context, filename string, base64 []byte) (string, error)
	DeleteFile(ctx context.Context, fileName string) error
}

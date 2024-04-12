package file

import (
	"context"
)

type Storage interface {
	GetFile(ctx context.Context, fileName string) (*File, error)
	GetFilesByNoteUUID(ctx context.Context) ([]*File, error)
	CreateFile(ctx context.Context, file *File) error
	DeleteFile(ctx context.Context, fileName string) error
}

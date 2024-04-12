package file

import (
	"context"
	"github.com/theartofdevel/notes_system/file_service/pkg/logging"
)

var _ Service = &service{}

type service struct {
	storage Storage
	logger  logging.Logger
}

func NewService(noteStorage Storage, logger logging.Logger) (Service, error) {
	return &service{
		storage: noteStorage,
		logger:  logger,
	}, nil
}

type Service interface {
	GetFile(ctx context.Context, fileName string) (f *File, err error)
	GetFilesByNoteUUID(ctx context.Context) ([]*File, error)
	Create(ctx context.Context, dto []byte) (string, error)
	Delete(ctx context.Context, fileName string) error
}

func (s *service) GetFile(ctx context.Context, fileId string) (f *File, err error) {
	f, err = s.storage.GetFile(ctx, fileId)
	if err != nil {
		return f, err
	}
	return f, nil
}

func (s *service) GetFilesByNoteUUID(ctx context.Context) ([]*File, error) {
	files, err := s.storage.GetFilesByNoteUUID(ctx)
	if err != nil {
		return nil, err
	}
	return files, nil
}

func (s *service) Create(ctx context.Context, dto []byte) (string, error) {
	fileName, err := NormalizeName(dto)
	if err != nil {
		return "", err
	}
	key, err := s.storage.CreateFile(ctx, fileName.String(), dto)
	if err != nil {
		return "", err
	}
	return key, nil
}

func (s *service) Delete(ctx context.Context, fileName string) error {
	err := s.storage.DeleteFile(ctx, fileName)
	if err != nil {
		return err
	}
	return nil
}

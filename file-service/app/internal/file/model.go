package file

import (
	"crypto/sha256"
	"fmt"
	"github.com/google/uuid"
	"io"
	"io/ioutil"
)

type File struct {
	ID    string `json:"id"`
	Name  string `json:"name"`
	Size  int64  `json:"size"`
	Bytes []byte `json:"file"`
}

type CreateFileDTO struct {
	Name   string `json:"name"`
	Size   int64  `json:"size"`
	Reader io.Reader
}

func NormalizeName(base64 []byte) (uuid.UUID, error) {

	hash := sha256.Sum256(base64)
	uuid := uuid.NewSHA1(uuid.Nil, hash[:])
	return uuid, nil
}

func NewFile(dto CreateFileDTO) (*File, error) {
	bytes, err := ioutil.ReadAll(dto.Reader)
	if err != nil {
		return nil, fmt.Errorf("failed to create file model. err: %w", err)
	}
	id, err := uuid.NewUUID()
	if err != nil {
		return nil, fmt.Errorf("failed to generate file id. err: %w", err)
	}

	return &File{
		ID:    id.String(),
		Name:  dto.Name,
		Size:  dto.Size,
		Bytes: bytes,
	}, nil
}

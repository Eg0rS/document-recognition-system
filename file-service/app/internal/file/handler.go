package file

import (
	"bytes"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"github.com/julienschmidt/httprouter"
	"github.com/theartofdevel/notes_system/file_service/internal/apperror"
	"github.com/theartofdevel/notes_system/file_service/pkg/logging"
	"io/ioutil"
	"net/http"
)

const (
	filesURL = "/api/files"
	fileURL  = "/api/files/:id"
)

type Handler struct {
	Logger      logging.Logger
	FileService Service
}

type ImageData struct {
	Image string `json:"image"`
}

func (h *Handler) Register(router *httprouter.Router) {
	router.HandlerFunc(http.MethodGet, fileURL, apperror.Middleware(h.GetFile))
	router.HandlerFunc(http.MethodGet, filesURL, apperror.Middleware(h.GetFilesByNoteUUID))
	router.HandlerFunc(http.MethodPost, filesURL, apperror.Middleware(h.CreateFile))
	router.HandlerFunc(http.MethodDelete, fileURL, apperror.Middleware(h.DeleteFile))
}

func (h *Handler) GetFile(w http.ResponseWriter, r *http.Request) error {
	h.Logger.Info("GET FILE")

	h.Logger.Debug("get note_uuid from URL")

	h.Logger.Debug("get fileId from context")
	params := r.Context().Value(httprouter.ParamsKey).(httprouter.Params)
	fileId := params.ByName("id")

	f, err := h.FileService.GetFile(r.Context(), fileId)
	if err != nil {
		return err
	}

	w.Header().Set("Content-Disposition", fmt.Sprintf("attachment; filename=%s", f.Name))
	w.Header().Set("Content-Type", r.Header.Get("Content-Type"))
	base64String := base64.StdEncoding.EncodeToString(f.Bytes)
	decodeString, err := base64.StdEncoding.DecodeString(base64String)
	if err != nil {
		return err
	}
	w.Write(decodeString)

	return nil
}

func (h *Handler) GetFilesByNoteUUID(w http.ResponseWriter, r *http.Request) error {
	h.Logger.Info("GET FILES BY NOTE UUID")
	w.Header().Set("Content-Type", "form/json")

	h.Logger.Debug("get note_uuid from URL")

	file, err := h.FileService.GetFilesByNoteUUID(r.Context())
	if err != nil {
		return err
	}

	filesBytes, err := json.Marshal(file)
	if err != nil {
		return err
	}

	w.WriteHeader(http.StatusOK)
	w.Write(filesBytes)

	return nil
}

func (h *Handler) CreateFile(w http.ResponseWriter, r *http.Request) error {
	h.Logger.Info("CREATE FILE")
	w.Header().Set("Content-Type", "form/json")

	body, err := ioutil.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Failed to read request body", http.StatusInternalServerError)
		return nil
	}

	// Декодируем JSON из тела запроса
	var imageData ImageData
	if err := json.Unmarshal(body, &imageData); err != nil {
		http.Error(w, "Failed to decode JSON", http.StatusBadRequest)
		return nil
	}

	// Проверяем наличие строки изображения в запросе
	if imageData.Image == "" {
		http.Error(w, "Image data is missing", http.StatusBadRequest)
		return nil
	}

	decodedBytes, err := base64.StdEncoding.DecodeString(imageData.Image)
	if err != nil {
		return err
	}
	key, err := h.FileService.Create(r.Context(), decodedBytes)
	if err != nil {
		return err
	}

	keyBytes, err := json.Marshal(key)
	if err != nil {
		return err
	}
	w.Write(bytes.Trim(keyBytes, "\""))
	w.WriteHeader(http.StatusCreated)

	return nil
}

func (h *Handler) DeleteFile(w http.ResponseWriter, r *http.Request) error {
	h.Logger.Info("DELETE FILE")
	w.Header().Set("Content-Type", "application/json")

	h.Logger.Debug("get fileId from context")
	params := r.Context().Value(httprouter.ParamsKey).(httprouter.Params)
	fileId := params.ByName("id")

	err := h.FileService.Delete(r.Context(), fileId)
	if err != nil {
		return err
	}
	w.WriteHeader(http.StatusNoContent)

	return nil
}
